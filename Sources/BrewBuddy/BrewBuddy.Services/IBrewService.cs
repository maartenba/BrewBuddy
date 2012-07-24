using System.Collections.Generic;
using BrewBuddy.Entities;
using BrewBuddy.Entities.Constants;

namespace BrewBuddy.Services
{
    public interface IBrewService
    {
        IEnumerable<Brew> GetBrews(string username);
        Brew GetBrew(int id);
        IEnumerable<Brew> GetPublicBrews();
        Brew GetPublicBrew(int id);
        Brew CreateBrew(string username, int recipeId, string title, string annotations, BrewStatus status);
        void UpdateBrew(string username, int id, string title, string annotations, BrewStatus status);
        void DeleteBrew(int id);
        void MakePublic(int id);
        void MakePrivate(int id);

        void LinkSensor(int id, string sensorId);
        void UnlinkSensor(int id, string sensorId);
    }
}