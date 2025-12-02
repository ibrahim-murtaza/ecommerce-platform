using ECommerce.Models;
using System.Collections.Generic;

namespace ECommerce.BLL.Interfaces
{
    public interface IUserService
    {
        List<User> GetAllUsers();
        User GetUserById(int id);
        User GetUserByEmail(string email);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int id);
        bool ValidateUserCredentials(string email, string passwordHash);
    }
}
