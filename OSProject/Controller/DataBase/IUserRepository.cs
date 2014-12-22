using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSProject.Model.Structures;

namespace OSProject.Controller.DataBase
{
    public interface IUserRepository:IDisposable
    {
        IEnumerable<User> GetUsers();
        IEnumerable<UserInfo> GetUserInfos();
    }
}
