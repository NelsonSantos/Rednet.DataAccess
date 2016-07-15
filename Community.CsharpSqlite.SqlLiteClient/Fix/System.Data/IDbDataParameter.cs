namespace System.Data
{
    public interface IDbDataParameter : IDataParameter
    {
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        byte Precision { get; set; }
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        byte Scale { get; set; }
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        int Size { get; set; }
    }
}