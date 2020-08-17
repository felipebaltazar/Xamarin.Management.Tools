using System.Diagnostics;

namespace Xamarin.Management.Tools.Exceptions
{
    public sealed class PotentialDeadlockException : DeadlockException
    {
        internal PotentialDeadlockException(StackTrace blockingStackTrace)
            : base(blockingStackTrace, "The blocking operation encountered a potential deadlock") { }
    }
}
