namespace System.Data
{
    public interface IDbCommand : IDisposable
    {
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        string CommandText { get; set; }
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        int CommandTimeout { get; set; }
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        CommandType CommandType { get; set; }
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        IDbConnection Connection { get; set; }
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        IDataParameterCollection Parameters { get; }
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        IDbTransaction Transaction { get; set; }
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        UpdateRowSource UpdatedRowSource { get; set; }

        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        void Cancel();
        //
        // Summary:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        IDbDataParameter CreateParameter();
        //
        // Summary:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        int ExecuteNonQuery();
        //
        // Summary:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        IDataReader ExecuteReader();
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   behavior:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        IDataReader ExecuteReader(CommandBehavior behavior);
        //
        // Summary:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        object ExecuteScalar();
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        void Prepare();
    }
}