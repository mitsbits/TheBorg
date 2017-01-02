using System;

namespace Borg.Infra.Postal
{
    public class EmailFormatException : Exception
    {
        public EmailFormatException(string email) : base($"{email} is not well formated email.")
        {
        }
    }
}