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
            FieldInfo fi = Id.GetType().GetField(Id.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
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
