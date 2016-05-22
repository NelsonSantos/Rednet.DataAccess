using System;
using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// A resolver that will serialize all properties, and ignore custom TypeConverter attributes.
/// </summary>
public class SerializableContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
{
    protected override IList<Newtonsoft.Json.Serialization.JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var properties = base.CreateProperties(type, memberSerialization);

        //foreach (var p in properties)
        //    p.Ignored = false;

        return properties;
    }

    protected override Newtonsoft.Json.Serialization.JsonContract CreateContract(Type objectType)
    {
        var contract = base.CreateContract(objectType);

        if (contract is Newtonsoft.Json.Serialization.JsonStringContract)
            return CreateObjectContract(objectType);
        return contract;
    }
}
