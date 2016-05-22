using System;

namespace Rednet.DataAccess
{
    public delegate void ErrorOnValidateDataEventHandler(object sender, ErrorOnValidateDataEventArgs e);

    public class ErrorOnValidateDataEventArgs : EventArgs
    {
        public ErrorOnValidateDataEventArgs(ValidatedField[] validatedFields)
        {
            ValidatedFields = validatedFields;
        }

        public ValidatedField[] ValidatedFields { get; internal set; }
    }

    public class ValidatedField
    {
        public string FieldName { get; internal set; }
        public string FieldMessage { get; internal set; }
    }
}