using System.Reflection;

namespace System.Data
{
    //[DefaultMember("Item")]
    public interface IDataRecord
    {
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   name:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        object this[string name] { get; }
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   i:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        object this[int i] { get; }

        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        int FieldCount { get; }

        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   i:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        bool GetBoolean(int i);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   i:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        byte GetByte(int i);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   i:
        //     To be added.
        //
        //   fieldOffset:
        //     To be added.
        //
        //   buffer:
        //     To be added.
        //
        //   bufferoffset:
        //     To be added.
        //
        //   length:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   i:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        char GetChar(int i);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   i:
        //     To be added.
        //
        //   fieldoffset:
        //     To be added.
        //
        //   buffer:
        //     To be added.
        //
        //   bufferoffset:
        //     To be added.
        //
        //   length:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   i:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        IDataReader GetData(int i);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   i:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        string GetDataTypeName(int i);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   i:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        DateTime GetDateTime(int i);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   i:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        decimal GetDecimal(int i);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   i:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        double GetDouble(int i);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   i:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        Type GetFieldType(int i);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   i:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        float GetFloat(int i);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   i:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        Guid GetGuid(int i);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   i:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        short GetInt16(int i);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   i:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        int GetInt32(int i);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   i:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        long GetInt64(int i);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   i:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        string GetName(int i);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   name:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        int GetOrdinal(string name);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   i:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        string GetString(int i);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   i:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        object GetValue(int i);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   values:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        int GetValues(object[] values);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   i:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        bool IsDBNull(int i);
    }

    //
    // Summary:
    //     To be added.
    //
    // Remarks:
    //     To be added.
}