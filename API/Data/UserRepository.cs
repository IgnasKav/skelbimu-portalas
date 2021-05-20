using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext context1;
        public UserRepository(UserContext context)
        {
            context1 = context;
        }
        public User Create(User user)
        {
            context1.Users.Add(user);
            user.Id = context1.SaveChanges();
            return user;
        }
    }
}
