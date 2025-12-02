using ECommerce.BLL.Interfaces;
using ECommerce.DAL;
using ECommerce.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.BLL.SPImplementation
{
    public class UserServiceSP : IUserService
    {
        private readonly ECommerceContext _context;

        public UserServiceSP(ECommerceContext context)
        {
            _context = context;
        }

        public List<User> GetAllUsers()
        {
            return _context.Users
                .FromSqlRaw("SELECT * FROM [User]")
                .ToList();
        }

        public User GetUserById(int id)
        {
            var idParam = new SqlParameter("@UserID", id);

            return _context.Users
                .FromSqlRaw("SELECT * FROM [User] WHERE UserID = @UserID", idParam)
                .AsEnumerable()
                .FirstOrDefault()!;
        }

        public User GetUserByEmail(string email)
        {
            var emailParam = new SqlParameter("@Email", email);

            return _context.Users
                .FromSqlRaw("SELECT * FROM [User] WHERE Email = @Email", emailParam)
                .AsEnumerable()
                .FirstOrDefault()!;
        }

        public void AddUser(User user)
        {
            var parameters = new[]
            {
                new SqlParameter("@Email", user.Email),
                new SqlParameter("@PasswordHash", user.PasswordHash),
                new SqlParameter("@FirstName", user.FirstName),
                new SqlParameter("@LastName", user.LastName),
                new SqlParameter("@Address", user.Address),
                new SqlParameter("@City", user.City)
            };

            _context.Database.ExecuteSqlRaw(
                "INSERT INTO [User] (Email, PasswordHash, FirstName, LastName, Address, City) " +
                "VALUES (@Email, @PasswordHash, @FirstName, @LastName, @Address, @City)",
                parameters);
        }

        public void UpdateUser(User user)
        {
            var parameters = new[]
            {
                new SqlParameter("@UserID", user.UserID),
                new SqlParameter("@Email", user.Email),
                new SqlParameter("@PasswordHash", user.PasswordHash),
                new SqlParameter("@FirstName", user.FirstName),
                new SqlParameter("@LastName", user.LastName),
                new SqlParameter("@Address", user.Address),
                new SqlParameter("@City", user.City)
            };

            _context.Database.ExecuteSqlRaw(
                "UPDATE [User] SET Email = @Email, PasswordHash = @PasswordHash, FirstName = @FirstName, " +
                "LastName = @LastName, Address = @Address, City = @City WHERE UserID = @UserID",
                parameters);
        }

        public void DeleteUser(int id)
        {
            var idParam = new SqlParameter("@UserID", id);
            _context.Database.ExecuteSqlRaw("DELETE FROM [User] WHERE UserID = @UserID", idParam);
        }

        public bool ValidateUserCredentials(string email, string passwordHash)
        {
            var parameters = new[]
            {
                new SqlParameter("@Email", email),
                new SqlParameter("@PasswordHash", passwordHash)
            };

            var user = _context.Users
                .FromSqlRaw("SELECT * FROM [User] WHERE Email = @Email AND PasswordHash = @PasswordHash", parameters)
                .AsEnumerable()
                .FirstOrDefault();

            return user != null;
        }
    }
}