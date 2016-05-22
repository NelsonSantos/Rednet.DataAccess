# Rednet.DataAccess

Rednet.DataAccess is one more another component to work with data that simplifies your use.

In my long journey to work with data, I have used much data access libraries to simplify the development, but in some moment, this libraries always presents a problem which is difficult to solve, and in most cases I always have to code SQL queries for data retrieving and performance.

The process of coding SQL queries always force me to remember of column and tables names that is all the time changing in our projects.

With my solution, we will define the classes with some of your properties with attributes that indicate our preferences on the tables and columns, and execute CRUD operations and data queries with a strong typing approach, and in the background it will generate the appropriate connection and SQL statement for us.

Ok, go to samples!

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

Well, here we define a class User inheriting from `DatabaseObject<>` class with four fields and setting the Id property with `FieldDefAttribute` and his `IsPrimaryKey` field to `true`.

Inheriting from `DatabaseObject<>` our class `User` now has some new methods.

Go to add some rows with the instance `SaveChanges` method :
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

Here we will retrieve the data with the static `Load` method:
```C#
var _user = User.Load(u => u.Id = 1);

Console.WriteLine(_user.Name);

// output -> 'Nelson'
```

Like the `Load` method, the `Query` method can retrieve a list of `User` class.

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