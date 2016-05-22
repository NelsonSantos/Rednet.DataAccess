using System;

namespace Rednet.DataAccess
{
    public enum FieldCheck
    {
        None,
        Equal,
        NotEqual,
        Range,
        LessThan,
        GreaterThan,
        Null,
        NotNull,
        NullOrEmpty
    }

    public class FieldRuleAttribute : Attribute
    {
        internal string Name { get; set; }
        public string ValidationText { get; set; }
        internal bool IsForValidate { get; set; }
        //public bool IsOk { get; internal set; }
        public FieldCheck CheckType { get; set; }
        internal Type Type { get; set; }
        public object FirstValueToCheck { get; set; }
        public object LastValueToCheck { get; set; }

        public bool Validate(object fieldValue)
        {
            var _isOk = false;
            switch (this.CheckType)
            {
                case FieldCheck.None:
                    _isOk = true;
                    break;

                case FieldCheck.NotEqual:
                    if (!fieldValue.Equals(this.FirstValueToCheck))
                        _isOk = true;
                    break;

                case FieldCheck.Equal:
                    if (fieldValue.Equals(this.FirstValueToCheck))
                        _isOk = true;
                    break;

                case FieldCheck.Range:
                    var _type = this.Type.Name.Replace("System.", "");

                    if (_type.ToLower() == "datetime")
                    {
                        var _first = (DateTime)this.FirstValueToCheck;
                        var _last = (DateTime)this.LastValueToCheck;
                        var _value = (DateTime)fieldValue;
                        if (_value >= _first && _value <= _last)
                            _isOk = true;
                    }
                    else
                    {
                        if ((decimal)fieldValue >= (decimal)this.FirstValueToCheck && (decimal)fieldValue <= (decimal)LastValueToCheck)
                            _isOk = true;
                    }
                    break;

                case FieldCheck.GreaterThan:
                    if ((decimal)fieldValue > (decimal)this.FirstValueToCheck)
                    {
                        _isOk = true;
                    }
                    break;

                case FieldCheck.LessThan:
                    if ((decimal)fieldValue < (decimal)this.FirstValueToCheck)
                    {
                        _isOk = true;
                    }
                    break;

                case FieldCheck.Null:
                    if (fieldValue == null)
                    {
                        _isOk = true;
                    }
                    break;

                case FieldCheck.NotNull:
                    if (fieldValue != null)
                    {
                        _isOk = true;
                    }
                    break;

                 case FieldCheck.NullOrEmpty:
                    if (fieldValue != null)
                    {
                        if (!fieldValue.ToString().Equals(""))
                            _isOk = true;
                    }
                    break;

            }

            //this.IsOk = _isOk;
            return _isOk;

        }
    }
}