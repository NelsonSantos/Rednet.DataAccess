using System;
using System.Collections.Generic;
#if !PCL 
using System.Data;
#endif
using System.Diagnostics;
#if !PCL && !WINDOWS_PHONE_APP
using System.IO.Compression;
using System.Security.Cryptography;
#endif
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

#if DROID
using Android.App;
#endif
#if __IOS__
using Foundation;
#endif
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnicodeNormalization;

public static class Extensions
{

#if !PCL
    private static Dictionary<Type, DbType> m_TypeMap = new Dictionary<Type, DbType>
            {
                {typeof (byte), DbType.Byte}, 
                {typeof (sbyte), DbType.SByte}, 
                {typeof (short), DbType.Int16}, 
                {typeof (ushort), DbType.UInt16}, 
                {typeof (int), DbType.Int32}, 
                {typeof (uint), DbType.UInt32}, 
                {typeof (long), DbType.Int64}, 
                {typeof (ulong), DbType.UInt64}, 
                {typeof (float), DbType.Single}, 
                {typeof (double), DbType.Double}, 
                {typeof (decimal), DbType.Decimal}, 
                {typeof (bool), DbType.Boolean}, 
                {typeof (string), DbType.String}, 
                {typeof (char), DbType.StringFixedLength}, 
                {typeof (Guid), DbType.Guid}, 
                {typeof (DateTime), DbType.DateTime}, 
                {typeof (DateTimeOffset), DbType.DateTimeOffset}, 
                {typeof (byte[]), DbType.Binary}, 
                {typeof (byte?), DbType.Byte}, 
                {typeof (sbyte?), DbType.SByte}, 
                {typeof (short?), DbType.Int16}, 
                {typeof (ushort?), DbType.UInt16}, 
                {typeof (int?), DbType.Int32}, 
                {typeof (uint?), DbType.UInt32}, 
                {typeof (long?), DbType.Int64}, 
                {typeof (ulong?), DbType.UInt64}, 
                {typeof (float?), DbType.Single}, 
                {typeof (double?), DbType.Double}, 
                {typeof (decimal?), DbType.Decimal}, 
                {typeof (bool?), DbType.Boolean}, 
                {typeof (char?), DbType.StringFixedLength}, 
                {typeof (Guid?), DbType.Guid}, 
                {typeof (DateTime?), DbType.DateTime}, 
                {typeof (DateTimeOffset?), DbType.DateTimeOffset}
            };

    public static DbType GetDbType(this Type type)
    {
        return m_TypeMap[type];
    }
#endif

    /// <summary>
    /// Determine whether a type is simple (String, Decimal, DateTime, etc) 
    /// or complex (i.e. custom class with public properties and methods).
    /// </summary>
    /// <see cref="http://stackoverflow.com/questions/2442534/how-to-test-if-type-is-primitive"/>
    public static bool IsSimpleType(
        this Type type)
    {
#if !PCL && !WINDOWS_PHONE_APP
        return
            type.IsValueType ||
            type.IsPrimitive ||
            new Type[] { 
				typeof(String),
				typeof(Decimal),
				typeof(DateTime),
				typeof(DateTimeOffset),
				typeof(TimeSpan),
				typeof(Guid)
			}.Contains(type) ||
            System.Convert.GetTypeCode(type) != TypeCode.Object;
#else
        return false;
#endif
    }

    public static Version GetAppVersion()
    {
        Version _ret = null;
#if __IOS__
        var _code = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"].ToString();
        var _v = _code.Split('.');
        _ret = new Version(
            int.Parse(_v[0]),
            int.Parse(_v[1]),
            int.Parse(_v[2]),
            int.Parse(_v[3])
            );
#elif DROID
        var _context = Application.Context.ApplicationContext;
        var _code = _context.PackageManager.GetPackageInfo(_context.PackageName, 0).VersionName;
        var _v = _code.Split('.');
        _ret = new Version(
            int.Parse(_v[0]),
            int.Parse(_v[1]),
            int.Parse(_v[2]),
            int.Parse(_v[3])
            );
#elif !PCL && !DROID && !__IOS__ && !WINDOWS_PHONE_APP
        var _assembly = Assembly.GetCallingAssembly();
        FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(_assembly.Location);
        var _v = fvi.FileVersion.Split('.');
        _ret = new Version(
            int.Parse(_v[0]),
            int.Parse(_v[1]),
            int.Parse(_v[2]),
            int.Parse(_v[3])
            );
#elif WINDOWS_PHONE_APP
        var _object = new object();
        return _object.GetType().GetTypeInfo().Assembly.GetName().Version;
#endif
        return _ret;
    }

    public static Stream GetStreamFromFile(string path)
    {
#if !PCL && !WINDOWS_PHONE_APP
        return new FileStream(path, FileMode.Open);
#elif WINDOWS_PHONE_APP
        return Windows.Storage.StorageFile.GetFileFromPathAsync(path).GetResults().OpenStreamForReadAsync().Result;
#else
        return Stream.Null;
#endif
    }

    public static void SaveBytesToFile(this byte[] values, string path)
    {
#if !PCL
        try
        {
#if WINDOWS_PHONE_APP
            var _file = Windows.Storage.StorageFile.GetFileFromPathAsync(path).GetResults();
            using (var _writer = _file.OpenStreamForWriteAsync().Result)
#else
            using (var _writer = new FileStream(path, FileMode.CreateNew))
#endif
            {
                _writer.Write(values, 0, values.Length);
                _writer.Flush();
#if !WINDOWS_PHONE_APP
                _writer.Close();
#endif
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
#endif
            }

    public static string ToMd5(this string value)
    {
#if !PCL
        byte[] _buffer = Encoding.UTF8.GetBytes(value);
        var _md5 = Md5.Create();
        byte[] _hash = _md5.ComputeHash(_buffer);

        var _ret = new StringBuilder();
        for (int i = 0; i < _hash.Length; i++)
        {
            _ret.Append(_hash[i].ToString("x2"));
        }

        return _ret.ToString();

#else
        return "";
#endif
    }

    public static string ReturnMD5Code(string FilePath)
    {

#if !PCL
        try
        {

#if WINDOWS_PHONE_APP
            var _file = Windows.Storage.StorageFile.GetFileFromPathAsync(FilePath).GetResults();
            if (_file != null)
#else
            var _file = new System.IO.FileInfo(FilePath);
            if (_file.Exists)
#endif
            {
                var _md5 = Md5.Create();

#if WINDOWS_PHONE_APP
                using (var _reader = new System.IO.BinaryReader(_file.OpenStreamForReadAsync().Result))

#else
                using (var _reader = new System.IO.BinaryReader(_file.Open(FileMode.Open)))
#endif
                {
                    var _buffer = new byte[_reader.BaseStream.Length];
                    _reader.Read(_buffer, 0, _buffer.Length);
#if !WINDOWS_PHONE_APP
                    _reader.Close();
#endif
                    var _hash = _md5.ComputeHash(_buffer);

                    var _ret = new System.Text.StringBuilder();
                    for (int i = 0; i < _hash.Length; i++)
                    {
                        _ret.Append(_hash[i].ToString("X2"));
                    }

                    return _ret.ToString();
                }

            }
            else
                return "";
        }
        catch (Exception ex)
        {
            return "";
        }
#else
        return "";
#endif
    }

    public static string RemoveAccents(this string text)
    {
#if !PCL
        return string.Concat(text.Normalize(UnicodeNormalization.NormalizationForm.FormD).Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)).Normalize(UnicodeNormalization.NormalizationForm.FormC);
#else
        return "";
#endif
    }


    public static string CompressString(this string text)
    {
#if !PCL
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        var memoryStream = new MemoryStream();
        using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
        {
            gZipStream.Write(buffer, 0, buffer.Length);
        }

        memoryStream.Position = 0;

        var compressedData = new byte[memoryStream.Length];
        memoryStream.Read(compressedData, 0, compressedData.Length);

        var gZipBuffer = new byte[compressedData.Length + 4];
        Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
        Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);

        return System.Convert.ToBase64String(gZipBuffer);
#else
        return "";
#endif

    }

    public static string DecompressString(this string compressedText)
    {
#if !PCL
        if (compressedText.Trim().Length == 0) return "";

        byte[] gZipBuffer = System.Convert.FromBase64String(compressedText);
        using (var memoryStream = new MemoryStream())
        {
            int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
            memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

            var buffer = new byte[dataLength];

            memoryStream.Position = 0;
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                gZipStream.Read(buffer, 0, buffer.Length);
            }

            return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
        }
#else
        return "";
#endif
    }

    public static string ConvertToString(this byte[] b)
    {
        var _ret = "";
#if !PCL && !WINDOWS_PHONE_APP
        _ret = Encoding.ASCII.GetString(b);
#elif WINDOWS_PHONE_APP
        _ret = Encoding.UTF8.GetString(b, 0, b.Length);
#endif
        if (!_ret.Equals(""))
        {
            var _pos = _ret.IndexOf((char) 0);

            _ret = _pos <= 0 ? "" : _ret.Substring(0, _pos);
        }

        return _ret;
    }

    public static char[] ConvertToChar(this byte[] b)
    {
#if !PCL && !WINDOWS_PHONE_APP
        return Encoding.ASCII.GetChars(b);
#elif WINDOWS_PHONE_APP
        return Encoding.UTF8.GetChars(b);
#else
        return new char[] {};
#endif
    }

    public static byte[] ConvertToByteArray(this string s)
    {
#if !PCL && !WINDOWS_PHONE_APP
        return Encoding.ASCII.GetBytes(s);
#elif WINDOWS_PHONE_APP
        return Encoding.UTF8.GetBytes(s);
#else
        return new byte[] {};
#endif
    }

    public static byte ToByte(this char c)
    {
#if !PCL && !WINDOWS_PHONE_APP
        return Encoding.ASCII.GetBytes(new char[] { c })[0];
#elif WINDOWS_PHONE_APP
        return Encoding.UTF8.GetBytes(new char[] { c })[0];
#else
        return new byte();
#endif
    }

    public static byte[] ConvertToByteArray(this char[] chrs)
    {
#if !PCL && !WINDOWS_PHONE_APP
        return Encoding.ASCII.GetBytes(chrs);
#elif WINDOWS_PHONE_APP
        return Encoding.UTF8.GetBytes(chrs);
#else
        return new byte[] {};
#endif
    }

    public static char ToChar(this byte[] b)
    {
#if !PCL
        return ConvertToChar(b)[0];
#else
        return new char();
#endif
    }

    public static List<T> Clone<T>(this List<T> list)
    {
        return JsonConvert.DeserializeObject<List<T>>(ToJson(list), new JsonSerializerSettings() {Converters = {new NumberConverter(), new IsoDateTimeConverter() {DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff"}}});
    }

    public static List<TDest> Convert<TSource, TDest>(this List<TSource> list)
    {
        return JsonConvert.DeserializeObject<List<TDest>>(ToJson(list), new JsonSerializerSettings() {Converters = {new NumberConverter(), new IsoDateTimeConverter() {DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff"}}});
    }

    public static string ToJson<T>(T data, bool compress = false)
    {
        var _ret = JsonConvert.SerializeObject(data, new JsonSerializerSettings() { Converters = { new NumberConverter(), new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff" } } });

        if (compress)
            _ret = _ret.CompressString();

        return _ret;
    }

    public static string ToJson<T>(this IList<T> data, bool compress = false)
    {
        var _ret = JsonConvert.SerializeObject(data, new JsonSerializerSettings() { Converters = { new NumberConverter(), new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff" } } });

        if (compress)
            _ret = _ret.CompressString();

        return _ret;
    }

    public static string ZeroEsquerda(this int value, int QtdeZero)
    {
        return value.ToString().PadLeft(QtdeZero, '0');
    }

    public static int ToValue(this int? value)
    {
        return value.HasValue ? value.Value : 0;
    }

    public static double ToValue(this double? value)
    {
        return value.HasValue ? value.Value : 0;
    }

    public static char ToValue(this char? value)
    {
        return value.HasValue ? value.Value : ' ';
    }

    public static bool ToValue(this bool? value)
    {
        return value.HasValue ? value.Value : false;
    }

    public static DateTime ToValue(this DateTime? value)
    {
        return value.HasValue ? value.Value : new DateTime();
    }

    /// <summary>
    /// Converte o double em int
    /// </summary>
    /// <param name="value">Valor que será convertido</param>
    /// <returns>Um valor int convertido a partir do double</returns>
    public static int ToInt(this double value)
    {
        return (int) value;
    }

    public static int ToInt(this float value)
    {
        return (int) value;
    }

    public static double ToDouble(this int value)
    {
        return System.Convert.ToDouble(value.ToString());
    }

    /// <summary>
    /// Converte a string em int
    /// </summary>
    /// <param name="value">Valor que será convertido</param>
    /// <returns>Um valor int convertido a partir da string</returns>
    public static int ToInt(this string value)
    {
        int _ret = 0;

        if (value != null)
            int.TryParse(value, out _ret);

        return _ret;
    }

    public static long ToLong(this string value)
    {
        long _ret = 0;

        if (value != null)
            long.TryParse(value, out _ret);

        return _ret;
    }

    public static decimal PortugueseBRToDecimal(this string Value)
    {
        return decimal.Parse(Value, GetPtBrCultureInfo().NumberFormat);
    }

    public static double ToDouble(this string Value)
    {
        return ToDouble(Value, false);
    }

    public static bool IsNullOrEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static double ApplyPercent(this double value, double percent)
    {
        var _tax = percent / 100d;
        var _value = value * _tax;
        var _ret = value + _value;
        return _ret;
    }

    public static string ToFormattedText(this double Value, int DecimalPlaces)
    {
        return ToFormattedText((double?) Value, DecimalPlaces);
    }

    public static string ToFormattedText(this double? Value, int DecimalPlaces)
    {
        string _format = "N" + DecimalPlaces.ToString();

        return string.Format(CultureInfo.CurrentCulture.NumberFormat, "{0:" + _format + "}", Value);
    }

    public static double ToDouble(this string Value, bool TryPortugueseBRCulture)
    {
        if (Value.IsNullOrEmpty()) return 0d;

        double _ret = 0;

        if (!TryPortugueseBRCulture)
            _ret = double.Parse(Value, CultureInfo.InvariantCulture.NumberFormat);
        else
        {
            if (!double.TryParse(Value, out _ret))
                _ret = PortugueseBRToDouble(Value);
        }
        return _ret;
    }

    private static CultureInfo GetPtBrCultureInfo()
    {
        return new CultureInfo("pt-BR");
    }

    public static double PortugueseBRToDouble(this string Value)
    {
        return double.Parse(Value, GetPtBrCultureInfo().NumberFormat);
    }

    public static decimal ToDecimal(this string Value)
    {
        return ToDecimal(Value, false);
    }

    public static decimal ToDecimal(this string Value, bool TryPortugueseBRCulture)
    {
        decimal _ret = 0;

        if (!TryPortugueseBRCulture)
            _ret = decimal.Parse(Value, CultureInfo.InvariantCulture.NumberFormat);
        else
        {
            if (!decimal.TryParse(Value, out _ret))
                _ret = PortugueseBRToDecimal(Value);
        }
        return _ret;
    }

    public static string ToStringBrasil(this DateTime value, bool showTime = true, bool showSeconds = true)
    {
        var _format = "dd/MM/yyyy" + (showTime ? (showSeconds ? " HH:mm:ss" : " HH:mm") : "");

        return value.ToString(_format);
    }

    /// <summary>
    /// Retorna a string baseada na quantidade de caracteres especificados
    /// </summary>
    /// <param name="s">Variavel a ser trabalhada</param>
    /// <param name="LenghtToRemove">Quantidade de carecteres a ser lido</param>
    /// <param name="Rest">Retorna o valor da varivel que foi trabalhada sem a string que foi retornada</param>
    /// <returns></returns>
    public static string GetStringAndRemove(this string s, int LenghtToRemove, out string Rest)
    {
        string _ret = s.Substring(0, LenghtToRemove);

        Rest = s.Remove(0, LenghtToRemove);

        return _ret;
    }

    public static double TransformToDouble(this string s, int QuantityDecimals)
    {
        int _div_number = int.Parse("1" + (QuantityDecimals == 0 ? "" : "".PadRight(QuantityDecimals, '0')));

        int _number = 0;
        int _out = 0;

        if (s.Length == 0)
            return 0;
        else
        {
            if (int.TryParse(s, out _out))
                _number = _out;
            else
                _number = 0;
        }

        double _ret = (double) _number/(double) _div_number;

        return _ret;
    }

    public static int TransformToInt(this string s)
    {
        if (s.Length == 0)
            return 0;
        else
        {
            int _ret = 0;
            if (int.TryParse(s, out _ret))
                return _ret;
            else
                return 0;
        }
    }

    public static long TransformToLong(this string s)
    {
        if (s.Length == 0)
            return 0;
        else
        {
            long _ret = 0;
            if (long.TryParse(s, out _ret))
                return _ret;
            else
                return 0;
        }
    }

    /// <summary>
    /// Retorna um inteiro baseado em uma string em formato hexadecimal
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static int FromHexNumberToInt(this string s)
    {
        return int.Parse(s, System.Globalization.NumberStyles.HexNumber);
    }

    public static string ToHexNumber(this byte b)
    {
        return b.ToString("X").PadLeft(2, '0');
    }

    public static string ToHexNumberPadding(this byte b)
    {
        return b.ToString("X").PadLeft(2, '0');
    }

    public static string ToHexNumber(this int i)
    {
        return i.ToString("X");
    }

    public static string ToHexNumberPadding(this int i)
    {
        return i.ToString("X").PadLeft(2, '0');
    }

    public static byte ToByte(this string s)
    {
        return byte.Parse(s);
    }

    public static string[] Split(this string s, int MaxLenghtToSplit)
    {

        List<string> _ret = new List<string>();

        for (int i = 0; i <= s.Length - 1; i += MaxLenghtToSplit)
        {
            _ret.Add(s.Substring(i, MaxLenghtToSplit));
        }

        return _ret.ToArray();
    }

    public static byte[] SplitInBytes(this int i)
    {
        string[] _splits = i.ToString().Split(1);

        List<byte> _bytes = new List<byte>();

        foreach (string _s in _splits)
        {
            _bytes.Add(byte.Parse(_s));
        }

        return _bytes.ToArray();
    }

    public static string ConvertToString(this string[] values)
    {
        var _ret = new StringBuilder();

        foreach (string _str in values)
        {
            _ret.Append(_str);
        }

        return _ret.ToString();

    }

    public static bool ToBool(this string value)
    {
        var _ret = false;

        return bool.TryParse(value, out _ret) && _ret;
    }

}
