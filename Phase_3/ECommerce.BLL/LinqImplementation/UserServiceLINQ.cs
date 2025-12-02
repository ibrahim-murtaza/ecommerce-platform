using ECommerce.BLL.Interfaces;
using ECommerce.DAL;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.BLL.LinqImplementation
{
    public class UserServiceLINQ : IUserService
    {
        private readonly ECommerceContext _context;

        public UserServiceLINQ(ECommerceContext context)
        {
            _context = context;
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User GetUserById(int id)
        {
            return _context.Users
            .Include(u => u.Orders)
            .Include(u => u.Carts)
            .FirstOrDefault(u => u.UserID == id)!;
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users
            .FirstOrDefault(u => u.Email == email)!;
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        public bool ValidateUserCredentials(string email, string passwordHash)
        {
            return _context.Users
                .Any(u => u.Email == email && u.PasswordHash == passwordHash);
        }
    }
}
