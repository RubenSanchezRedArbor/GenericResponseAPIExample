﻿
namespace ExampleAPI.Common
{
    public class NoContentException : Exception
    {
        public NoContentException(string message) : base(message)
        {

        }
    }
}
