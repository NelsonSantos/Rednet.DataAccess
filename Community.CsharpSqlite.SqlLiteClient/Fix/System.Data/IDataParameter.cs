namespace System.Data
{
    public interface IDataParameter
    {
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        DbType DbType { get; set; }
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        ParameterDirection Direction { get; set; }
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        bool IsNullable { get; }
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        string ParameterName { get; set; }
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        string SourceColumn { get; set; }
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        DataRowVersion SourceVersion { get; set; }
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        object Value { get; set; }
    }
}