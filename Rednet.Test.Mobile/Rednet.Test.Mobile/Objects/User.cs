using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Rednet.DataAccess;

namespace Rednet.Test.Mobile.Objects
{

    public enum UserType
    {
        Simple,
        Administrator
    }

    public class User : DatabaseObject<User>
    {
        [FieldDef(IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public UserType UserType { get; set; }
    }
}
