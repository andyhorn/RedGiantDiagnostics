using System;
using System.Collections.Generic;

namespace API.Exceptions
{
    public class ActionFailedException : Exception
    {
        public IEnumerable<string> Errors;
        public ActionFailedException() 
        {
            Errors = new List<string>();
        }

        public ActionFailedException(IEnumerable<string> errors)
        {
            Errors = errors;
        }
    }
}