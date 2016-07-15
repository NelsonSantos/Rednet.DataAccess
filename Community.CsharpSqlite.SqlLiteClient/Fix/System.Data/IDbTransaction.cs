namespace System.Data
{
    public interface IDbTransaction : IDisposable
    {
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        IDbConnection Connection { get; }
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        IsolationLevel IsolationLevel { get; }

        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        void Commit();
        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        void Rollback();
    }
}