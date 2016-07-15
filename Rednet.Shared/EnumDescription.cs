using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

public interface IEnumDescription<T>
{
    T Id { get; set; }
    string Description { get; }
    string ToString();
}

[AttributeUsage(AttributeTargets.All)]
public class DescriptionAttribute : Attribute
{
    public DescriptionAttribute(string description)
    {
        this.Description = description;
    }

    public string Description { get; set; }
}

#if !PCL
public class EnumDescription<T> : IEnumDescription<T>
{

    public EnumDescription(T id)
    {
        this.Id = id;
    }

    public T Id { get; set; }

    public string Description
    {
        get
        {
#if WINDOWS_PHONE_APP
            FieldInfo fi = Id.GetType().GetRuntimeField(Id.ToString());
#else
            FieldInfo fi = Id.GetType().GetField(Id.ToString());
#endif
            DescriptionAttribute[] _attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (_attributes != null &&
                _attributes.Length > 0)
                return _attributes[0].Description;
            else
                return Id.ToString();
        }
    }

    public override string ToString()
    {
        return this.Description;
    }

    public static List<EnumDescription<T>> GetFromEnumValues()
    {
        var _ret = new List<EnumDescription<T>>();

        foreach (var _enum in Enum.GetValues(typeof(T)))
        {
            _ret.Add(new EnumDescription<T>((T) _enum));
        }

        return _ret;
    }
}
#endif
