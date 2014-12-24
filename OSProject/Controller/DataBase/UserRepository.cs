using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSProject.Model.Structures;

namespace OSProject.Controller.DataBase
{
    public class UserRepository:IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository()
        {
            _context = new UserDbContext();
        }
        public IEnumerable<User> GetUsers()
        {
            return _context.Users;
        }

        public IEnumerable<UserInfo> GetUserInfos()
        {
            return _context.UserInfos;
        }
        public User GetUser(string login)
        {
            return _context.Users.Include("UserInfo").Where(x => x.login.Equals(login)).FirstOrDefault();

        }
        public bool AddUser(User user)
        {
            try
            {
                if (_context.Users.FirstOrDefault(x => x.login.Equals(user.login)) == null)
                    return false;
                _context.Users.Add(user);
                _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public bool RemoveUser(User user)
        {
            try
            {
                _context.Users.Remove(user);
                _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }


        #region Dispose
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
