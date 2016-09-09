using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Rednet.Test.Mobile
{
    public partial class UserList : ContentPage
    {
        public UserList()
        {
            InitializeComponent();
            this.BindingContext = new UserListModel();
        }
    }
}
