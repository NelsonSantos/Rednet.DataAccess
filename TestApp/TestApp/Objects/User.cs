using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rednet.DataAccess;

namespace TestApp.Objects
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
