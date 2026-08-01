using System;

namespace StudentManagementAPI.Exceptions
{
    public class BadRequestException : Exception
    {

        public BadRequestException(string message)
            : base(message)
        {

        }

    }
}
