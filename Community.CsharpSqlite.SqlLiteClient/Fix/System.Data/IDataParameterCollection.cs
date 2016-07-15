using System.Collections;

namespace System.Data
{
    public interface IDataParameterCollection : IList, ICollection, IEnumerable
    {
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   parameterName:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        object this[string parameterName] { get; set; }

        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   parameterName:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        bool Contains(string parameterName);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   parameterName:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        int IndexOf(string parameterName);
        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   parameterName:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        void RemoveAt(string parameterName);
    }
}