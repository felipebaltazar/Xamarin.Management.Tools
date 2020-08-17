using System;
using System.Threading;

namespace Xamarin.Management.Tools
{
    public class SyncContextTracker : IDisposable
    {
        public static bool GenerateStackTraces { get; set; }

        public static SyncContextTrackerMode DefaultMode { get; set; } = SyncContextTrackerMode.ReportActualDeadlocks;

        private readonly ManagedSynchronizationContext _managedSynchronizationContext;
        private readonly SyncContextTrackerMode _detectionMode;

        public static IDisposable Start() => Start(DefaultMode);

        public static IDisposable Start(SyncContextTrackerMode detectionMode)
        {
            if (detectionMode == SyncContextTrackerMode.None)
                return null;

            return new SyncContextTracker(detectionMode);
        }

        private SyncContextTracker(SyncContextTrackerMode detectionMode)
        {
            _detectionMode = detectionMode;

            var currentSynchronizationContext = SynchronizationContext.Current;
            if (currentSynchronizationContext == null && detectionMode == SyncContextTrackerMode.ReportActualDeadlocks)
                return;

            _managedSynchronizationContext = new ManagedSynchronizationContext(currentSynchronizationContext);
            SynchronizationContext.SetSynchronizationContext(_managedSynchronizationContext);
        }

        ~SyncContextTracker()
        {
            if (_detectionMode != SyncContextTrackerMode.None)
                throw new InvalidOperationException("Always dispose the sync tracker!");
        }

        void IDisposable.Dispose()
        {
            if (_managedSynchronizationContext != null)
                SynchronizationContext.SetSynchronizationContext(_managedSynchronizationContext.BaseSynchronizationContext);

            GC.SuppressFinalize(this);
        }
    }
}
