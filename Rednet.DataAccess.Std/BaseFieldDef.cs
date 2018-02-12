using System;

namespace Rednet.DataAccess
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class BaseFieldDef : Attribute
    {
        public bool DisplayOnForm { get; set; } = true;
        public bool DisplayOnGrid { get; set; } = true;
        public bool EditOnForm { get; set; } = true;
        public string Label { get; set; }
        public string ShortLabel { get; set; }
        public bool IsInternal { get; set; }
        public bool SerializeField { get; set; }
        public string Name { get; set; }
        public bool IgnoreForSave { get; set; }
    }
}