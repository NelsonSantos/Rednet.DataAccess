This library provides a MySQL driver for connecting to remote mysql server.

# Please don't use this library in your product.

# Hackers could either just sniff the traffic for credentials or disassemble the app to get them.

If you use it in your app, use it at your own risk.

Hoverver, if your app is only for personal use or don't care about sercurity issue, you can use this library to connect to MySQL directly without Web/REST API server.

## Examples 1

```csharp
using MySql.Data.MySqlClient;
...

public void GetAccountCountFromMySQL()
{
	try
	{
		new I18N.West.CP1250 ();
		MySqlConnection sqlconn;
		string connsqlstring = "Server=your.ip.address;Port=3306;database=YOUR_DATA_BASE;User Id=root;Password=password;charset=utf8";
		sqlconn = new MySqlConnection(connsqlstring);
		sqlconn.Open();
		string queryString = "select count(0) from ACCOUNT";
		MySqlCommand sqlcmd = new MySqlCommand(queryString, sqlconn);
		String result = sqlcmd.ExecuteScalar ().ToString();
		LblMsg.Text = result + " accounts in DB";
		sqlconn.Close();
	} catch (Exception ex)
	{
		Console.WriteLine (ex.Message);
	}
}
```

##### Step 1.
Setup your MySQL database, and create an ACCOUNT data table in your database.
##### Step 2.
Replace IP address, port, user, and password with yours in the connsqlstring.
##### Step 3.
Build and deploy your app.

## Examples 2

```csharp

public List<String> LoadAllItemFromMySQL()
{
	List<String> products = new List<String>();
	try{
		string connsqlstring = "Server=your.ip.address;Port=3306;database=YOUR_DATA_BASE;User Id=root;Password=password;charset=utf8";
		MySqlConnection sqlconn = new MySqlConnection (connsqlstring);
		sqlconn.Open ();

		DataSet tickets = new DataSet();
		string queryString = "select item.NAME from ITEM as item";
		MySqlDataAdapter adapter = new MySqlDataAdapter(queryString, sqlconn);
		adapter.Fill(tickets, "Item");
		foreach(DataRow row in tickets.Tables["Item"].Rows) {
			products.Add(row[0].ToString());
		}

		sqlconn.Close ();
	} catch(Exception e) {
		Console.Write (e.Message);
	}
	return products;
}
```
