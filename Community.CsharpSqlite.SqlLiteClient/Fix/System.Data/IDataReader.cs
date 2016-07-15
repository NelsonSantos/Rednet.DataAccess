namespace System.Data
{
    public interface IDataReader : IDisposable, IDataRecord
    {
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        int Depth { get; }
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        bool IsClosed { get; }
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        int RecordsAffected { get; }

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
        DataTable GetSchemaTable();
        //
        // Summary:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        bool NextResult();
        //
        // Summary:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        bool Read();
    }

    //
    // Summary:
    //     To be added.
    //
    // Remarks:
    //     To be added.
}