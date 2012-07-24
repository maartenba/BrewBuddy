using System.Linq;
using System.Transactions;
using BrewBuddy.Entities;

namespace BrewBuddy.Services
{
    public class UserService
        : IUserService
    {
        protected IEntityRepository<UserProfile> ProfileRepository { get; private set; }

        public UserService(IEntityRepository<UserProfile> profileRepository)
        {
            ProfileRepository = profileRepository;
        }

        public UserProfile GetProfile(string username)
        {
            return ProfileRepository.GetAll().FirstOrDefault(p => p.UserName == username);
        }

        public UserProfile CreateDefaultProfile(string username, string email)
        {
            return CreateProfile(username, email, username, "", "");
        }

        public UserProfile CreateProfile(string username, string email, string name, string location, string information)
        {
            var profile = new UserProfile()
                              {
                                  UserName = username,
                                  Email = email,
                                  Name = name,
                                  Location = location,
                                  Information = information
                              };

            ProfileRepository.InsertOnCommit(profile);
            ProfileRepository.CommitChanges();

            return profile;
        }

        public void UpdateProfile(string username, string email, string name, string location, string information)
        {
            var profile = GetProfile(username);
            if (profile != null)
            {
                profile.Email = email;
                profile.Name = name;
                profile.Location = location;
                profile.Information = information;

                ProfileRepository.CommitChanges();
            }
        }
    }
}