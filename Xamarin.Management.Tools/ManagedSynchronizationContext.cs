using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using Xamarin.Management.Tools.Exceptions;

namespace Xamarin.Management.Tools
{
    [SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.ControlPolicy | SecurityPermissionFlag.ControlEvidence)]
    internal class ManagedSynchronizationContext : SynchronizationContext
    {
        private readonly object _sync = new object();

        private StackTrace stacktrace;
        private bool isBlocking;

        public SynchronizationContext BaseSynchronizationContext { get; }

        public ManagedSynchronizationContext(SynchronizationContext baseSynchronizationContext)
        {
            BaseSynchronizationContext = baseSynchronizationContext;
            SetWaitNotificationRequired();
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            lock (_sync)
            {
                if (isBlocking)
                {
                    if (BaseSynchronizationContext != null)
                        throw new PotentialDeadlockException(stacktrace);

                    throw new DeadlockException(stacktrace);
                }
            }

            BaseSynchronizationContext.Post(d, state);
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            if (BaseSynchronizationContext != null)
                BaseSynchronizationContext.Send(d, state);
            else
                base.Send(d, state);
        }

        public override void OperationStarted()
        {
            if (BaseSynchronizationContext != null)
                BaseSynchronizationContext.OperationStarted();
            else
                base.OperationStarted();
        }

        public override void OperationCompleted()
        {
            if (BaseSynchronizationContext != null)
                BaseSynchronizationContext.OperationCompleted();
            else
                base.OperationCompleted();
        }

        [SecurityCritical]
        public override int Wait(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
        {
            Debug.Assert(!isBlocking);

            try
            {
                lock (_sync)
                {
                    isBlocking = true;
                    if (SyncContextTracker.GenerateStackTraces)
                        stacktrace = new StackTrace();
                }
                var waitContext = BaseSynchronizationContext ?? this;
                return waitContext.Wait(waitHandles, waitAll, millisecondsTimeout);
            }
            finally
            {
                isBlocking = false;
            }
        }

        public override SynchronizationContext CreateCopy()
        {
            var copy = new ManagedSynchronizationContext(BaseSynchronizationContext?.CreateCopy());
            lock (_sync)
            {
                copy.isBlocking = isBlocking;
                copy.stacktrace = stacktrace;
            }
            return copy;
        }
    }
}
