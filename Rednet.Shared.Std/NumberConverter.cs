using System;
using Newtonsoft.Json;

public class NumberConverter : JsonConverter
{
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        object _ret = null;

        if (objectType == typeof (Decimal))
            _ret = reader.Value.ToString().ToDecimal(true);
        else if (objectType == typeof (Double))
            _ret = reader.Value.ToString().ToDouble(true);

        return _ret;
    }

    public override bool CanWrite
    {
        get { return false; }
    }

    public override bool CanConvert(Type objectType)
    {
        if (objectType == typeof (decimal))
            return true;
        else if (objectType == typeof (double))
            return true;
        else
            return false;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

}