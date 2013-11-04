using System;
using System.Reflection;


namespace SuperSimple.Auth
{
    [Serializable]
    public class InvalidKeyException : Exception
    {
        public InvalidKeyException(){}
        public InvalidKeyException(string message): base(message){}
    }

    public class DuplicateUserException : Exception
    {
        public DuplicateUserException(){}
        public DuplicateUserException(string message): base(message){}
    }

    public class AuthenticationFailedException : Exception
    {
        public AuthenticationFailedException(){}
        public AuthenticationFailedException(string message): base(message){}
    }

    public class InvalidTokenException : Exception
    {
        public InvalidTokenException(){}
        public InvalidTokenException(string message): base(message){}
    }
}

