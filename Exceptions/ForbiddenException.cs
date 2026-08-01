using System;

namespace StudentManagementAPI.Exceptions
{
    public class ForbiddenException : Exception
    {

        public ForbiddenException(string message)
            : base(message)
        {

        }

    }
}
