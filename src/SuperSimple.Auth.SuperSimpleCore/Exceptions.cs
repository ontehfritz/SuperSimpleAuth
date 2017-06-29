namespace SuperSimple.Auth
{
    using System;

    public class InvalidException : Exception
    {
        public InvalidException(){}
        public InvalidException(string message): base(message){}
    }

    public class DuplicateException : Exception
    {
        public DuplicateException(){}
        public DuplicateException(string message): base(message){}
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(){}
        public UnauthorizedException(string message): base(message){}
    }
}

