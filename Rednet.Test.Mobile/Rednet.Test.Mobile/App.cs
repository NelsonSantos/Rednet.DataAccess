using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using Rednet.DataAccess;
using Rednet.Test.Mobile.Objects;
using Xamarin.Forms;

namespace Rednet.Test.Mobile
{
    public class App : Application
    {
        //private static IFolder m_Root = Device.OS == TargetPlatform.WinPhone ? FileSystem.Current.RoamingStorage : FileSystem.Current.LocalStorage;
        private static IFolder m_Root = FileSystem.Current.LocalStorage;
        private StackLayout m_MainStack;

        public App()
        {

            m_MainStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label {HorizontalTextAlignment = TextAlignment.Center, Text = "Rednet DataAccess Test App!"},
                }
            };

            // The root page of your application
            MainPage = new ContentPage
            {
                Title = "TestApp",
                Content = m_MainStack
            };

            this.Initialization = this.Init();
        }

        private async Task Init()
        {
            //setting the database

            // setting the database path
            var _dataBasePath = PortablePath.Combine(m_Root.Path, "Databases");
            //var _dataBasePath = "Databases";

            if (await m_Root.CheckExistsAsync(_dataBasePath) == ExistenceCheckResult.NotFound)
            {
                var _temp = await m_Root.CreateFolderAsync(_dataBasePath, CreationCollisionOption.OpenIfExists);
            }

            // setting the db file
            //var _file = Path.Combine(_dataBasePath, "database.db3");
            var _file = "database.db3";

            if (await m_Root.CheckExistsAsync(_file) == ExistenceCheckResult.NotFound)
            {
                var _retfile = await m_Root.CreateFileAsync(_file, CreationCollisionOption.ReplaceExisting);
            }

            // create the database connection function
            var _dbFuncName = "MyDataFunctionName";
            _file = PortablePath.Combine(m_Root.Path, _file);
            var _function = new DataFunctionsSQLite(_file) { Name = _dbFuncName };

            DatabaseObjectShared.DataFunctions.Clear();
            DatabaseObjectShared.DataFunctions.Add(_dbFuncName, _function);
            DatabaseObjectShared.DefaultDataFunctionName = _dbFuncName;

            //creating table User
            VerifyObject<User>(_function, _dataBasePath);
            VerifyObject<Purchase>(_function, _dataBasePath);
            VerifyObject<PurchaseItem>(_function, _dataBasePath);

            // creating the screen that will do the examples

            // insert a record
            var _add = new Button { Text = "Add new User Record" };
            _add.Clicked += (sender, args) =>
            {
                // adding a new record on User table
                // getting total number of rows on table
                var _count = _function.ExecuteScalar<long>("select count(0) from User") + 1;

                // create a new instance of User class
                var _user = new User { Id = (int)_count, Name = string.Format("User #{0}", _count), Password = "xyz", UserType = _count == 1 ? UserType.Administrator : UserType.Simple };

                // save data on db
                _user.SaveChanges();
            };

            // open up a list of User`s
            var _list = new Button { Text = "Open User List" };
            _list.Clicked += async (sender, args) =>
            {
                await App.Current.MainPage.Navigation.PushModalAsync(new UserList());
            };

            // delete a row
            var _delete = new Button { Text = "Delete last row on table" };
            _delete.Clicked += (sender, args) =>
            {

                // list all rows and get last User row
                var _users = User.Query(); //--> null predicates return all rows from table
                var _user = _users.LastOrDefault();

                if (_user != null)
                    _user.Delete();
            };

            // delete all admin Users
            var _deleteOnlyAdm = new Button { Text = "Delete all Admin Users" };
            _deleteOnlyAdm.Clicked += (sender, args) =>
            {
                User.DeleteAll(u => u.UserType == UserType.Administrator);
            };

            // delete all simple Users
            var _deleteOnlySimple = new Button { Text = "Delete all Simple Users" };
            _deleteOnlySimple.Clicked += (sender, args) =>
            {
                User.DeleteAll(u => u.UserType == UserType.Simple);
            };

            // delete all rows
            var _deleteAll = new Button { Text = "Delete all rows on table" };
            _deleteAll.Clicked += (sender, args) =>
            {
                // for all rows parameters must be null
                User.DeleteAll();
            };

            // search for Nelson
            var _searchFor = new Button { Text = "Search for Nelson..." };
            _searchFor.Clicked += (sender, args) =>
            {
                // for all rows parameters must be null
                foreach (var _item in User.Query(u => u.Name.Contains("nelson")))
                {
                    Debug.WriteLine("User Name: {0}", _item.Name);
                }
            };

            var _jsonData = "{ Id : 9999, Name : \"Nelson Santos\", Password : \"123\", UserType : 1 }"; // UserType -> 0 = Simple / 1 = Administrator

            var _loadFromJson = new Button { Text = "Load from Json data\r\r" + _jsonData };
            _loadFromJson.Clicked += (sender, args) =>
            {
                var _user = User.FromJson(_jsonData);
                _user.SaveChanges();
            };

            for (var _id = 1; _id < 3; _id++)
            {
                var _order = new Purchase { PurchaseId = _id, PurchaseDate = DateTime.Parse("2016-05-25") };
                var _saved = _order.SaveChanges();

                var _amount = 1;
                for (int _item = 1; _item < (_id == 1 ? 4 : 3); _item++)
                {
                    var _orderitem = new PurchaseItem { PurchaseId = _id, ItemId = _item, Amount = _amount, Price = 1.0 };
                    _orderitem.SaveChanges();
                    _amount++;
                }
            }

            var _orders = Purchase.Query(); // get all rows on table

            foreach (var _order in _orders)
            {
                Debug.WriteLine(string.Format("Purchase: {0} - PurchaseDate: {1} - Total: {2}", _order.PurchaseId, _order.PurchaseDate, _order.TotalPurchase));
                foreach (var _item in _order.OrderItems)
                {
                    Debug.WriteLine("Item: {0} - Amount: {1} - Price: {2} - Total: {3}", _item.ItemId, _item.Amount, _item.Price, _item.TotalItem);
                }
            }

            // adding the button on screen
            m_MainStack.Children.Add(_add);
            m_MainStack.Children.Add(_list);
            m_MainStack.Children.Add(_delete);
            m_MainStack.Children.Add(_deleteOnlyAdm);
            m_MainStack.Children.Add(_deleteOnlySimple);
            m_MainStack.Children.Add(_deleteAll);
            m_MainStack.Children.Add(_searchFor);
            m_MainStack.Children.Add(_loadFromJson);
        }

        private Task Initialization { get; }

        public static void VerifyObject<TDatabaseObject>(IDataFunctions func, string path) where TDatabaseObject : IDatabaseObject
        {
            try
            {
                if (func.CheckDdlScript<TDatabaseObject>(path))
                    return;

                if (func.AlterTable<TDatabaseObject>())
                    func.SaveDdlScript<TDatabaseObject>(path);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
