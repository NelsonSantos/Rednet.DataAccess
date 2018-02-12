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

        public ValidatedField[] ValidatedFields { get; }
    }

    public class ValidatedField
    {
        public string FieldName { get; set; }
        public string FieldMessage { get; set; }
    }
}