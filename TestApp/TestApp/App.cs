using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using Rednet.DataAccess;
using TestApp.Objects;
using Xamarin.Forms;

namespace TestApp
{
    public class App : Application
    {
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
            var _dataBasePath = Path.Combine(m_Root.Path, "Databases");
            var _checkFolder = await m_Root.CheckExistsAsync(_dataBasePath);

            if (_checkFolder != ExistenceCheckResult.FolderExists)
            {
                await m_Root.CreateFolderAsync(_dataBasePath, CreationCollisionOption.FailIfExists);
            }

            // setting the db file
            var _file = Path.Combine(_dataBasePath, "database.db3");
            var _checkFile = await m_Root.CheckExistsAsync(_file);

            if (_checkFile != ExistenceCheckResult.FileExists)
            {
                var _retfile = m_Root.CreateFileAsync(_file, CreationCollisionOption.ReplaceExisting).Result;
            }

            // create the database connection function
            var _dbFuncName = "MyDataFunctionName";
            var _function = new DataFunctionsSQLite() { DatabaseFile = _file, Name = _dbFuncName };

            DatabaseObjectShared.DataFunctions.Clear();
            DatabaseObjectShared.DataFunctions.Add(_dbFuncName, _function);
            DatabaseObjectShared.DefaultDataFunctionName = _dbFuncName;

            //creating table User
            VerifyObject<User>(_function, _dataBasePath);

            // creating the screen that will do the examples
            var _add = new Button {Text = "Add new User Record"};
            _add.Clicked += (sender, args) =>
            {
                // adding a new record on User table
                // getting total number of rows on table
                var _count = _function.ExecuteScalar<long>("select count(0) from User") + 1;

                // create a new instance of User class
                var _user = new User {Id = (int)_count, Name = string.Format("User #{0}", _count), Password = "xyz", UserType = _count == 1 ? UserType.Administrator : UserType.Simple};

                // save data on db
                _user.SaveChanges();
            };

            var _list = new Button {Text = "Open User List"};
            _list.Clicked += async (sender, args) =>
            {
                await App.Current.MainPage.Navigation.PushModalAsync(new UserList());
            };

            // The root page of your application
            m_MainStack.Children.Add(_add);
            m_MainStack.Children.Add(_list);
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
