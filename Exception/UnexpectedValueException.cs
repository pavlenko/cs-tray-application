using System;

namespace TrayApplication.Exception
{
    public class UnexpectedValueException : SystemException
    {
        public UnexpectedValueException(object value, string type) : base(FormatMessage(value, type))
        {}

        private static string FormatMessage(object value, string type)
        {
            return string.Format(
                "Expected argument of type {0}, {1} given",
                type,
                value.GetType().FullName
            );
        }
    }
}