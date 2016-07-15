using System.Runtime.InteropServices;
using Platform;

namespace System
{

    public enum TypeCode
    {
        //
        // Summary:
        //     To be added.
        Empty = 0,
        //
        // Summary:
        //     To be added.
        Object = 1,
        //
        // Summary:
        //     To be added.
        DBNull = 2,
        //
        // Summary:
        //     To be added.
        Boolean = 3,
        //
        // Summary:
        //     To be added.
        Char = 4,
        //
        // Summary:
        //     To be added.
        SByte = 5,
        //
        // Summary:
        //     To be added.
        Byte = 6,
        //
        // Summary:
        //     To be added.
        Int16 = 7,
        //
        // Summary:
        //     To be added.
        UInt16 = 8,
        //
        // Summary:
        //     To be added.
        Int32 = 9,
        //
        // Summary:
        //     To be added.
        UInt32 = 10,
        //
        // Summary:
        //     To be added.
        Int64 = 11,
        //
        // Summary:
        //     To be added.
        UInt64 = 12,
        //
        // Summary:
        //     To be added.
        Single = 13,
        //
        // Summary:
        //     To be added.
        Double = 14,
        //
        // Summary:
        //     To be added.
        Decimal = 15,
        //
        // Summary:
        //     To be added.
        DateTime = 16,
        //
        // Summary:
        //     To be added.
        String = 18
    }

    //
    // Summary:
    //     Defines methods that convert the value of the implementing reference or value
    //     type to a common language runtime type that has an equivalent value.    [CLSCompliant(false)]
    [ComVisible(true)]
    public interface IConvertible
    {
        //
        // Summary:
        //     Returns the System.TypeCode for this instance.
        //
        // Returns:
        //     The enumerated constant that is the System.TypeCode of the class or value type
        //     that implements this interface.
        TypeCode GetTypeCode();
        //
        // Summary:
        //     Converts the value of this instance to an equivalent Boolean value using the
        //     specified culture-specific formatting information.
        //
        // Parameters:
        //   provider:
        //     An System.IFormatProvider interface implementation that supplies culture-specific
        //     formatting information.
        //
        // Returns:
        //     A Boolean value equivalent to the value of this instance.
        bool ToBoolean(IFormatProvider provider);
        //
        // Summary:
        //     Converts the value of this instance to an equivalent 8-bit unsigned integer using
        //     the specified culture-specific formatting information.
        //
        // Parameters:
        //   provider:
        //     An System.IFormatProvider interface implementation that supplies culture-specific
        //     formatting information.
        //
        // Returns:
        //     An 8-bit unsigned integer equivalent to the value of this instance.
        byte ToByte(IFormatProvider provider);
        //
        // Summary:
        //     Converts the value of this instance to an equivalent Unicode character using
        //     the specified culture-specific formatting information.
        //
        // Parameters:
        //   provider:
        //     An System.IFormatProvider interface implementation that supplies culture-specific
        //     formatting information.
        //
        // Returns:
        //     A Unicode character equivalent to the value of this instance.
        char ToChar(IFormatProvider provider);
        //
        // Summary:
        //     Converts the value of this instance to an equivalent System.DateTime using the
        //     specified culture-specific formatting information.
        //
        // Parameters:
        //   provider:
        //     An System.IFormatProvider interface implementation that supplies culture-specific
        //     formatting information.
        //
        // Returns:
        //     A System.DateTime instance equivalent to the value of this instance.
        DateTime ToDateTime(IFormatProvider provider);
        //
        // Summary:
        //     Converts the value of this instance to an equivalent System.Decimal number using
        //     the specified culture-specific formatting information.
        //
        // Parameters:
        //   provider:
        //     An System.IFormatProvider interface implementation that supplies culture-specific
        //     formatting information.
        //
        // Returns:
        //     A System.Decimal number equivalent to the value of this instance.
        decimal ToDecimal(IFormatProvider provider);
        //
        // Summary:
        //     Converts the value of this instance to an equivalent double-precision floating-point
        //     number using the specified culture-specific formatting information.
        //
        // Parameters:
        //   provider:
        //     An System.IFormatProvider interface implementation that supplies culture-specific
        //     formatting information.
        //
        // Returns:
        //     A double-precision floating-point number equivalent to the value of this instance.
        double ToDouble(IFormatProvider provider);
        //
        // Summary:
        //     Converts the value of this instance to an equivalent 16-bit signed integer using
        //     the specified culture-specific formatting information.
        //
        // Parameters:
        //   provider:
        //     An System.IFormatProvider interface implementation that supplies culture-specific
        //     formatting information.
        //
        // Returns:
        //     An 16-bit signed integer equivalent to the value of this instance.
        short ToInt16(IFormatProvider provider);
        //
        // Summary:
        //     Converts the value of this instance to an equivalent 32-bit signed integer using
        //     the specified culture-specific formatting information.
        //
        // Parameters:
        //   provider:
        //     An System.IFormatProvider interface implementation that supplies culture-specific
        //     formatting information.
        //
        // Returns:
        //     An 32-bit signed integer equivalent to the value of this instance.
        int ToInt32(IFormatProvider provider);
        //
        // Summary:
        //     Converts the value of this instance to an equivalent 64-bit signed integer using
        //     the specified culture-specific formatting information.
        //
        // Parameters:
        //   provider:
        //     An System.IFormatProvider interface implementation that supplies culture-specific
        //     formatting information.
        //
        // Returns:
        //     An 64-bit signed integer equivalent to the value of this instance.
        long ToInt64(IFormatProvider provider);
        //
        // Summary:
        //     Converts the value of this instance to an equivalent 8-bit signed integer using
        //     the specified culture-specific formatting information.
        //
        // Parameters:
        //   provider:
        //     An System.IFormatProvider interface implementation that supplies culture-specific
        //     formatting information.
        //
        // Returns:
        //     An 8-bit signed integer equivalent to the value of this instance.
        sbyte ToSByte(IFormatProvider provider);
        //
        // Summary:
        //     Converts the value of this instance to an equivalent single-precision floating-point
        //     number using the specified culture-specific formatting information.
        //
        // Parameters:
        //   provider:
        //     An System.IFormatProvider interface implementation that supplies culture-specific
        //     formatting information.
        //
        // Returns:
        //     A single-precision floating-point number equivalent to the value of this instance.
        float ToSingle(IFormatProvider provider);
        //
        // Summary:
        //     Converts the value of this instance to an equivalent System.String using the
        //     specified culture-specific formatting information.
        //
        // Parameters:
        //   provider:
        //     An System.IFormatProvider interface implementation that supplies culture-specific
        //     formatting information.
        //
        // Returns:
        //     A System.String instance equivalent to the value of this instance.
        string ToString(IFormatProvider provider);
        //
        // Summary:
        //     Converts the value of this instance to an System.Object of the specified System.Type
        //     that has an equivalent value, using the specified culture-specific formatting
        //     information.
        //
        // Parameters:
        //   conversionType:
        //     The System.Type to which the value of this instance is converted.
        //
        //   provider:
        //     An System.IFormatProvider interface implementation that supplies culture-specific
        //     formatting information.
        //
        // Returns:
        //     An System.Object instance of type conversionType whose value is equivalent to
        //     the value of this instance.
        object ToType(Type conversionType, IFormatProvider provider);
        //
        // Summary:
        //     Converts the value of this instance to an equivalent 16-bit unsigned integer
        //     using the specified culture-specific formatting information.
        //
        // Parameters:
        //   provider:
        //     An System.IFormatProvider interface implementation that supplies culture-specific
        //     formatting information.
        //
        // Returns:
        //     An 16-bit unsigned integer equivalent to the value of this instance.
        ushort ToUInt16(IFormatProvider provider);
        //
        // Summary:
        //     Converts the value of this instance to an equivalent 32-bit unsigned integer
        //     using the specified culture-specific formatting information.
        //
        // Parameters:
        //   provider:
        //     An System.IFormatProvider interface implementation that supplies culture-specific
        //     formatting information.
        //
        // Returns:
        //     An 32-bit unsigned integer equivalent to the value of this instance.
        uint ToUInt32(IFormatProvider provider);
        //
        // Summary:
        //     Converts the value of this instance to an equivalent 64-bit unsigned integer
        //     using the specified culture-specific formatting information.
        //
        // Parameters:
        //   provider:
        //     An System.IFormatProvider interface implementation that supplies culture-specific
        //     formatting information.
        //
        // Returns:
        //     An 64-bit unsigned integer equivalent to the value of this instance.
        ulong ToUInt64(IFormatProvider provider);
    }
}