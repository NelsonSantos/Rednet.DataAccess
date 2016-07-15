namespace System.Data
{
    public interface IDbConnection : IDisposable
    {
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        string ConnectionString { get; set; }
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        int ConnectionTimeout { get; }
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        string Database { get; }
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        ConnectionState State { get; }

        //
        // Summary:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        IDbTransaction BeginTransaction();
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   il:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        //IDbTransaction BeginTransaction(IsolationLevel il);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   databaseName:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        void ChangeDatabase(string databaseName);
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        void Close();
        //
        // Summary:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        IDbCommand CreateCommand();
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        void Open();
    }
}