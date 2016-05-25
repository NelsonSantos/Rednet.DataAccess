![][logo] 
# Rednet.DataAccess 
[logo]:http://www.rednetsoftware.com.br/avatar_rednet_nuget_small.png

### Setup
* Available on NuGet: https://www.nuget.org/packages/Rednet.DataAccess [![NuGet](https://img.shields.io/nuget/v/Rednet.DataAccess.svg?label=NuGet)](https://www.nuget.org/packages/Rednet.DataAccess/)

## For update news, follow me on twitter:
* [@nelson_santos](https://twitter.com/rednetsoftware)
* [@rednetsoftware](https://twitter.com/nelson_santos)

**Rednet.DataAccess is one more another component to work with data that simplifies your use.**

In my long journey to work with data, I have used lot of data access libraries to simplify the development, but in some moment, this libraries always presents a problem which is difficult to solve, and in most cases I always have to code SQL queries for data retrieving and performance.

The process of coding SQL queries always force me to remember of column and tables names that is all the time changing in our projects.

With Rednet.DataAccess, we will define the classes with some of your properties with attributes that indicate our preferences on the tables and columns, and execute CRUD operations and data queries with a strong typing approach, and in the background it will generate the appropriate connections and SQL statements for us.

**Ok, go to samples!**

Before use, we need to set up the database configuration. We need todo this only once, for example, on startup routine of our app.

`DatabaseObjectShared` is a class that will contains the parameters to connect on database.
`DataFunctionsSQLite` is one of the supported databases of the framework.
Currently supports **Oracle**, **MySQL** and **SQLite**. PostgreSQL and SQLServer soon.

```C#
var _file = Path.Combine(_dataBasePath, "RednetAccess.db3");
var _dbFuncName = "MyDataFunctionName";
var _function = new DataFunctionsSQLite() { DatabaseFile = _file, Name = _dbFuncName };

DatabaseObjectShared.DataFunctions.Clear();
DatabaseObjectShared.DataFunctions.Add(_dbFuncName, _function);
DatabaseObjectShared.DefaultDataFunctionName = _dbFuncName;
```

In this framework we have a generic class named `DatabaseObject<>` that encapsulate all the methods to work with data (querys and CRUD) in static and instance manner.
We will create our classes inheriting from him for that we can manipulate the data.

Defining our **User** class sample:

```C#
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
```

Well, here we define a class `User` inheriting from `DatabaseObject<>` class with four fields and setting the Id property with `FieldDefAttribute` and his `IsPrimaryKey` field to `true`. Even that your table does not have a primary key, you must define one of your properties to have one. This is important to have one that will be used to localize the records when this in CRUD operations.

Inheriting from `DatabaseObject<>` our class `User` now has some new methods.

**Go to add some rows with the instance `SaveChanges()` method:**
```C#
    var _user1 = new User { Id = 1, Name = 'Nelson', Password = 'xyz', UserType = UserType.Administrator };
    var _user2 = new User { Id = 2, Name = 'Robert', Password = 'xyz', UserType = UserType.Simple };
    var _user3 = new User { Id = 3, Name = 'Willian', Password = 'xyz', UserType = UserType.Simple };
    var _user4 = new User { Id = 4, Name = 'Thompson', Password = 'xyz', UserType = UserType.Simple };
    
    _user1.SaveChanges();
    _user2.SaveChanges();
    _user3.SaveChanges();
    _user4.SaveChanges();
```

The `SaveChanges()` method do a check on the record checking if it already exists on database table. If exists, do a update on it, otherwise is inserted.

**Here we will retrieve the data with the static `Load()` method:**
```C#
var _user = User.Load(u => u.Id = 1);

Console.WriteLine(_user.Name);

// output -> 'Nelson'
```

**Like the `Load()` method, the `Query()` method can retrieve a list of `User` class.**

```C#
var _users = User.Query(u => u.UserType = UserType.Simple);

foreach(var _user in _users)
{
    Console.WriteLine(_user.Name);
}
// output -> 'Robert'
// output -> 'Willian'
// output -> 'Thompson'
```

Both `Load()` and `Query()` methods use same mechanism to retrieve data. The difference is that `Load()` returns null when no data found, and `Query()` returns a empty list.

Rednet.DataAccess use one of first versions of **Dapper.Net** inside the framework, and we could use that approach on same methods to load data with a custom SQL statement.

```C#
var _users = User.Query("select * from Users Where UserType = @UserType", new {UserType = UserType.Simple});

foreach(var _user in _users)
{
    Console.WriteLine(_user.Name);
}
// output -> 'Robert'
// output -> 'Willian'
// output -> 'Thompson'
```

**The FieldDefAttribute**

The `FieldDefAttribute` can change the behavior of your class. He is a very important part of framework and indicates how some properties inside your class will be treated by the engine of Rednet.DataAccess. Below are some of main fields descriptions for `FieldDefAttribute`:

Property Name|Data type|Description
-------------|---------|-----------
AutomaticValue|Enum|This has three values<br>**None** = Default - Does nothing.<br>**AutoIncrement** = Indicate that properties with this type are backend calculate and will not be included in Insert fields statements. On **SQLite** database you can use it to generate auto increment columns.<br>**BackEndCalculated** = Properties with this type are treated like AutoIncrement value, but does not auto generate a value. Like object **Sequences** on **Oracle**, you need to make some work to put the new value on your column (like a trigger) and the Rednet.DataAccess will get it back to your object.
IsPrimaryKey|Boolean|Indicate that the property is part of a Primary Key Constraint. This can be applied on more than one property inside your class. In all classes that inherit from `DatabaseObject<>` must have at least one property decorated with `FieldDefAttribute` and setted with IsPrimaryKey = true to function properly.
IgnoreForSave|Boolean|Properties marked with this will be ignored from DML Statements inside the framework. Its useful when you need to create some properties that return some data that is generated in run time and are not present in your table.

### Populating inner objects

With Rednet.DataAccess we can populate inner objects that are present on our classes.  For that we need to use `JoinFieldAttribute` on the properties indicating to the framework that in the moment of generate the SQL statement that he will include a **inner**, **left** or **right** join command and populate the inner properties with the results. See the code:

```C#
    public class Purchase : DatabaseObject<Purchase>
    {
        [FieldDef(AutomaticValue = AutomaticValue.AutoIncrement, IsPrimaryKey = true)]
        public int PurchaseId { get; set; }

        public DateTime PurchaseDate { get; set; }

        [FieldDef(IgnoreForSave = true)]
        public double TotalPurchase
        {
            get { return this.OrderItems.Sum(i => i.TotalItem); }
        }

        [JoinField(SourceColumnNames = new [] { "OrderId" }, TargetColumnNames = new [] {"OrderId"}, JoinRelation = JoinRelation.OneToMany, JoinType = JoinType.LeftJoin)]
        public ObservableCollection<PurchaseItem> OrderItems { get; set; } 
    }
    
    public class PurchaseItem : DatabaseObject<PurchaseItem>
    {
        [FieldDef(IsPrimaryKey = true)]
        public int PurchaseId { get; set; }

        [FieldDef(IsPrimaryKey = true)]
        public int ItemId { get; set; }

        public int Amount { get; set; }

        public double Price { get; set; }

        [FieldDef(IgnoreForSave = true)]
        public double TotalItem
        {
            get
            {
                return this.Amount * this.Price;
            }
        }
    }   
```

**Put some data to test:**
```C#
    for (var _id = 1; _id < 3; _id++)
    {
        var _order = new Purchase { PurchaseId = _id, PurchaseDate = DateTime.Parse("2016-05-25") };
        _order.SaveChanges(); 
    
        var _amount = 1;
        for (int _item = 1; _item < (_id == 1 ? 4 : 3); _item++)
        {
            var _orderitem = new PurchaseItem { PurchaseId = _id, ItemId = _item, Amount = _amount, Price = 1.0 };
            _orderitem.SaveChanges();
            _amount++;
        }
    }
```

**One line of code, all your records...**
```C#
    var _orders = Purchase.Query(); // get all rows on table
```

**and, displaying then!**
```C#
    foreach (var _order in _orders)
    {
        Console.WriteLine(string.Format("Purchase: {0} - PurchaseDate: {1} - Total: {2}", _order.PurchaseId, _order.PurchaseDate, _order.TotalPurchase));
        foreach (var _item in _order.OrderItems)
        {
            Console.WriteLine("Item: {0} - Amount: {1} - Price: {2} - Total: {3}", _item.ItemId, _item.Amount, _item.Price, _item.TotalItem);
        }
    }
    
    //-> output
    //Purchase: 1 - PurchaseDate: 5/25/2016 12:00:00 AM - Total: 6
    //Item: 1 - Amount: 1 - Price: 1 - Total: 1
    //Item: 2 - Amount: 2 - Price: 1 - Total: 2
    //Item: 3 - Amount: 3 - Price: 1 - Total: 3
    //Purchase: 2 - PurchaseDate: 5/25/2016 12:00:00 AM - Total: 3
    //Item: 1 - Amount: 1 - Price: 1 - Total: 1
    //Item: 2 - Amount: 2 - Price: 1 - Total: 2    
```

**The `JoinFieldAttribute` main properties:**

Property Name|Type|Description
-------------|----|-----------
SourceColumnNames|String[]|A array with the source column names on current table/class object that will be connected with the foreign table/class.
TargetColumnNames|String[]|A array with the target column names of the foreign table/class object that will be connected with the source table/class. This list of column names must be the same of the type of decorated with `JoinFieldAttribute` property.
JoinRelation|Enum|Informs to framework how this data will be populate on property.<br>**OneToOne** = Informs that only one row of that type will be populated on the property.<br>**OneToMany** = Informs that zero or more than one row will be populated on the property. Usually this is used on properties of types IList<>, IObservables<>, IEnumerables<>, etc.
JoinType|Enum|Informs to the framework how the SQL statement will be constructed with relation to **Join** command.<br>**InnerJoin** = Selects all rows from both tables as long as there is a match between the columns in both tables.<br>**LeftJoin** = Selects all rows from the left table (main object class), with the matching rows in the right table (property on main object class). The result is NULL in the right side when there is no match.<br>**RightJoin** = Selects all rows from the right table (property on main object class), with the matching rows in the left table (main object class). The result is NULL in the left side when there is no match.

**Below some samples codes for delete of data:**
```C#

    // list all rows and get last User row
    var _users = User.Query(); //--> null predicates return all rows from table
    var _user = _users.LastOrDefault();

    if (_user != null)
        _user.Delete();

    // delete all admin Users
    User.DeleteAll(u => u.UserType == UserType.Administrator);

    // delete all simple Users
    User.DeleteAll(u => u.UserType == UserType.Simple);

    // delete all rows
    User.DeleteAll(); //--> for all rows predicate must be null

```

**Useful functions**

Below a list of useful functions to use with it:

Function Name|Type|Description
-------------|----|-----------
FromJson()|static|Transforms a json data in the corresponding object
ToJson()|instance|Export the object into json data format
Exists()|static|Check if indicated predicate on static object exist in the corresponding database table
Exists()|instance|Check if current data on instanced object exist on corresponding database table
Clone()|instance|Clone current object in a new instance object
CloneTo<TTarget>()|instance|Clone current object in a new instance generic object using its properties names to populate the new object.

**Some code samples:**

Json
```C#
    // load data from json string
    // UserType -> 0 = Simple / 1 = Administrator
    var _jsonData = "{ Id : 9999, Name : \"Nelson Santos\", Password : \"123\", UserType : 1 }"; 
    var _user = User.FromJson(_jsonData);
    _user.SaveChanges();

    _user.ToJson();
    
    //--> out put 
    //{ 
    //    Id : 9999, 
    //    Name : "Nelson Santos", 
    //    Password : "123", 
    //    UserType : 1 
    //}
```
Notes:
* `ToJson()` can be used with optional parameter `compressString` (defaults to false) for guiding the method to compress the data before its return.
* `FromJson()` can be used with optional parameter `decompressString` (defaults to false) for guiding the method to decompress the string before deserialize the data and return the new object.

Checking if data exist (static method)
```C#
    var _exists = User.Exists(u => u.Id == 1); // can return true or false
```

Checking if data exist (instance method)
```C#
    var _jsonData = "{ Id : 9999, Name : \"Nelson Santos\", Password : \"123\", UserType : 1 }"; 
    var _user = User.FromJson(_jsonData);
    var _exists = _user.Exists(); // can return true or false
```

Here he use the property decorated with FieldDefAttribute (Id) where IsPrimaryKey field is set to true to identify which columns to use on internal SQL statement.
