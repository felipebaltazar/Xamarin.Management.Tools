using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Management.Tools
{
    public enum SyncContextTrackerMode
    {
        /// <summary>
        /// No custom behavior will be apply to the synchronization context
        /// </summary>
        None,

        /// <summary>
        /// Synchronization context will report actual deadlocks only.
        /// </summary>
        /// <remarks>
        /// Only situations that will actually result in a deadlock situation
        /// will be reported (the <see cref="DeadlockException"/> will be
        /// thrown).
        /// </remarks>
        ReportActualDeadlocks,

        /// <summary>
        /// Synchronization context will report deadlocks and also report situation that might deadlock in different situations.
        /// </summary>
        /// <remarks>
        /// Situations that could lead to a deadlock situation when a
        /// <see cref="System.Threading.SynchronizationContext"/> is used will
        /// also be reported when no 
        /// <see cref="System.Threading.SynchronizationContext"/> is active.
        /// This can help you test your library in different circumstances.
        /// </remarks>
        ReportPotentialDeadlocks
    }
}
