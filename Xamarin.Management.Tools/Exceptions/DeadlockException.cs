using System;
using System.Diagnostics;
using System.Text;

namespace Xamarin.Management.Tools.Exceptions
{
    public class DeadlockException : SystemException
    {
        public StackTrace BlockingStackTrace { get; }

        internal DeadlockException(StackTrace blockingStackTrace, string defaultMessage = "The blocking operation encountered a deadlock.")
            : base(ParseMessage(blockingStackTrace, defaultMessage))
        {
            BlockingStackTrace = blockingStackTrace;
        }

        protected static string ParseMessage(StackTrace blockingStackTrace, string exceptionDescription)
        {
            var sb = new StringBuilder();
            sb.AppendLine(exceptionDescription);

            if (blockingStackTrace != null)
            {
                sb.AppendLine();
                sb.AppendLine("Stack trace where the blocking operation started:");
                sb.Append(blockingStackTrace);
            }

            return sb.ToString();
        }
    }
}
