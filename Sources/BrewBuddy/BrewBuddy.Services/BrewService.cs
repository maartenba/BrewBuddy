using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Text;
using System.Transactions;
using BrewBuddy.Entities;
using BrewBuddy.Entities.Constants;
using FluentACS.ManagementService;

namespace BrewBuddy.Services
{
    public class BrewService
        : IBrewService
    {
        private const string AcsNamespace = "brewbuddy-euwest-1-sb";
        private const string ManagementIssuer = "ManagementClient";
        private const string ManagementKey = "enter your management key here";

        protected IEntityRepository<Brew> BrewRepository { get; private set; }

        public BrewService(IEntityRepository<Brew> brewRepository)
        {
            BrewRepository = brewRepository;
        }

        public IEnumerable<Brew> GetBrews(string username)
        {
            return BrewRepository.GetAll().Where(r => r.UserName == username);
        }

        public Brew GetBrew(int id)
        {
            return BrewRepository.Get(id);
        }

        public IEnumerable<Brew> GetPublicBrews()
        {
            return BrewRepository.GetAll().Where(r => r.IsPublic);
        }

        public Brew GetPublicBrew(int id)
        {
            var brew = BrewRepository.Get(id);
            if (brew.IsPublic)
            {
                return brew;
            }

            return null;
        }

        public Brew CreateBrew(string username, int recipeId, string title, string annotations, BrewStatus status)
        {
            var brew = new Brew()
            {
                UserName = username,
                RecipeId = recipeId,
                Title = title,
                Annotations = annotations,
                Status = status,
                LastModified = DateTime.UtcNow
            };

            BrewRepository.InsertOnCommit(brew);
            BrewRepository.CommitChanges();

            return brew;
        }

        public void UpdateBrew(string username, int id, string title, string annotations, BrewStatus status)
        {
            var brew = GetBrew(id);
            if (brew != null && brew.UserName == username)
            {
                brew.Title = title;
                brew.Annotations = annotations;
                brew.Status = status;
                brew.LastModified = DateTime.UtcNow;

                BrewRepository.CommitChanges();
            }
        }

        public void DeleteBrew(int id)
        {
            BrewRepository.DeleteOnCommit(GetBrew(id));

            BrewRepository.CommitChanges();
        }

        public void MakePublic(int id)
        {
            var brew = GetBrew(id);
            brew.IsPublic = true;
            brew.LastModified = DateTime.UtcNow;
            BrewRepository.CommitChanges();
        }

        public void MakePrivate(int id)
        {
            var brew = GetBrew(id);
            brew.IsPublic = false;
            brew.LastModified = DateTime.UtcNow;
            BrewRepository.CommitChanges();
        }

        public void LinkSensor(int id, string sensorId)
        {
            // Has a sensor already been linked?
            var brewWithSensor = BrewRepository.GetAll().FirstOrDefault(b => b.SensorId == sensorId);
            if (brewWithSensor != null && brewWithSensor.Id != id)
            {
                throw new ArgumentException(
                    string.Format("The sensor with id {0} can not be linked to the brew because the sensor has already been linked to another brew.", sensorId), "sensorId");
            }

            // Get the brew
            var brew = GetBrew(id);

            // First unlink the current sensor
            if (!string.IsNullOrEmpty(brew.SensorId))
            {
                UnlinkSensor(id, brew.SensorId);
            }

            // Link sensor in our datastore
            brew.SensorId = sensorId;
            brew.LastModified = DateTime.UtcNow;
            BrewRepository.CommitChanges();

            // We want a custom identity for the sensor which only allows sending to the service bus.
            var serviceManagementWrapper = new ServiceManagementWrapper(AcsNamespace, ManagementIssuer, ManagementKey);
            var client = serviceManagementWrapper.CreateManagementServiceClient();
            client.IgnoreResourceNotFoundException = true;

            // Clean up if we already exist as a sensor
            var existingRule = client.Rules.AddQueryOption("$filter", "Description eq '" + string.Format("Add Send claim value for sensor id {0}", sensorId) + "'").FirstOrDefault();
            if (existingRule != null)
            {
                client.DeleteObject(existingRule);
                client.SaveChanges(SaveChangesOptions.Batch);
            }
            serviceManagementWrapper.RemoveServiceIdentity(sensorId);

            // Create a new identity
            var serviceIdentity = new ServiceIdentity
            {
                Name = sensorId,
                Description = string.Format("Sensor id: {0}", sensorId)
            };
            var serviceIdentityKey = new ServiceIdentityKey
            {
                DisplayName = string.Format("Credentials for {0}", sensorId),
                Value = Encoding.UTF8.GetBytes(sensorId),
                Type = IdentityKeyTypes.Symmetric.ToString(),
                Usage = IdentityKeyUsages.Password.ToString(),
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(2) // sensors can be linked for up to 2 months
            };

            // Process modifications to the namespace
            client.AddToServiceIdentities(serviceIdentity);
            client.AddRelatedObject(serviceIdentity, "ServiceIdentityKeys", serviceIdentityKey);
            client.SaveChanges(SaveChangesOptions.Batch);

            // Add a Send claim
            var issuer = client.Issuers.AddQueryOption("$filter", "Name eq 'LOCAL AUTHORITY'").FirstOrDefault();
            var ruleGroup = client.RuleGroups.AddQueryOption("$filter", "Name eq 'Default Rule Group for ServiceBus'").FirstOrDefault();
            var rule = new Rule
            {
                Description = string.Format("Add Send claim value for sensor id {0}", sensorId),
                InputClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                InputClaimValue = sensorId,
                OutputClaimType = "net.windows.servicebus.action",
                OutputClaimValue = "Send",
                IssuerId = issuer.Id,
                RuleGroupId = ruleGroup.Id,
                RuleGroup = ruleGroup,
                Issuer = issuer
            };
            client.AddToRules(rule);
            client.SaveChanges(SaveChangesOptions.Batch);
        }

        public void UnlinkSensor(int id, string sensorId)
        {
            // Disallow access to the service bus for this sensor
            var serviceManagementWrapper = new ServiceManagementWrapper(AcsNamespace, ManagementIssuer, ManagementKey);
            var client = serviceManagementWrapper.CreateManagementServiceClient();
            client.IgnoreResourceNotFoundException = true;

            // Clean up if we already exist as a sensor
            var existingRule = client.Rules.AddQueryOption("$filter", "Description eq '" + string.Format("Add Send claim value for sensor id {0}", sensorId) + "'").FirstOrDefault();
            if (existingRule != null)
            {
                client.DeleteObject(existingRule);
                client.SaveChanges(SaveChangesOptions.Batch);
            }
            serviceManagementWrapper.RemoveServiceIdentity(sensorId);

            // Unlink sensor in our datastore
            var brew = GetBrew(id);
            brew.SensorId = null;
            brew.LastModified = DateTime.UtcNow;
            BrewRepository.CommitChanges();
        }
    }
}