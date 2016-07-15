using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Rednet.DataAccess
{

    /// <summary>
    /// A resolver that will serialize all properties, and ignore custom TypeConverter attributes.
    /// </summary>
    public class SerializableContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
    {

        protected override IList<Newtonsoft.Json.Serialization.JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var _isAnonymous = type.Name.ToLower().Contains("anonymoustype");

            var _properties = base.CreateProperties(type, memberSerialization);

            if (_isAnonymous) return _properties;

            foreach (var _p in _properties)
            {
                var _att = _p.AttributeProvider.GetAttributes(false).FirstOrDefault(p => p.GetType() == typeof(JoinFieldAttribute));

                if (_att == null)
                    _p.Ignored = true;
                else
                {
                    if (!(_att as JoinFieldAttribute).SerializeField)
                        _p.Ignored = true;
                }
            }

            var _fields = TableDefinition.GetTableDefinition(type).Fields.Select(f => f.Value);

            foreach (var _field in _fields)
            {
                var _p = _properties.FirstOrDefault(p => p.PropertyName == _field.Name);
                if (_p != null)
                    _p.Ignored = false;
            }
            return _properties;
        }

        protected override Newtonsoft.Json.Serialization.JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);

            if (contract is Newtonsoft.Json.Serialization.JsonStringContract)
                return CreateObjectContract(objectType);
            return contract;
        }
    }
}