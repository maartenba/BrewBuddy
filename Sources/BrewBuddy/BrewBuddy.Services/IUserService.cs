using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BrewBuddy.Entities;

namespace BrewBuddy.Services
{
    public interface IUserService
    {
        UserProfile GetProfile(string username);
        UserProfile CreateDefaultProfile(string username, string email);
        UserProfile CreateProfile(string username, string email, string name, string location, string information);
        void UpdateProfile(string username, string email, string name, string location, string information);
    }
}
