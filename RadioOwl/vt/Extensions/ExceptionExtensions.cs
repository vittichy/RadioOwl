using System;
using System.Linq;
using System.Collections.Generic;

namespace vt.Extensions
{
    public static class ExceptionExtensions
    {
        public static List<string> GetExceptionMessages(this Exception ex)
        {
            var messages = ex.GetInnerExceptions().Select(p => p.Message).ToList();
            return messages;
        }


        public static List<Exception> GetInnerExceptions(this Exception ex)
        {
            var exceptionsSet = new List<Exception>();
            GetInnerExceptions(ref exceptionsSet, ex);
            return exceptionsSet;
        }


        private static void GetInnerExceptions(ref List<Exception> exceptionsSet, Exception ex)
        {
            if(ex != null)
            {
                exceptionsSet.Add(ex);
                GetInnerExceptions(ref exceptionsSet, ex.InnerException);
            }
        }

    }
}
