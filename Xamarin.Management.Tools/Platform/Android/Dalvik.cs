using System;
using System.Diagnostics.SymbolStore;
using System.Threading;
using System.Threading.Tasks;
using JavaClass = Java.Lang.Class;
using JavaMethod = Java.Lang.Reflect.Method;
using JavaObject = Java.Lang.Object;

namespace Xamarin.Management.Tools.Platform.Android
{
    /// <summary>
    /// Provides access to <see href="https://android.googlesource.com/platform/libcore-snapshot/+/refs/heads/ics-mr1/dalvik/src/main/java/dalvik/system/VMDebug.java">Dalvik VM</see>
    /// </summary>
    public sealed class Dalvik : IDisposable
    {
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        private static readonly JavaObject[] emptyArgs = new JavaObject[0];

        private static readonly JavaMethod _dumpGREFTableMethod =
            JavaClass.ForName("dalvik.system.VMDebug")
                     .GetDeclaredMethod("dumpReferenceTables");

        private static readonly JavaMethod _countInstancesOfClassMethod =
            JavaClass.ForName("dalvik.system.VMDebug")
                     .GetDeclaredMethod("countInstancesOfClass");

        public static void DumpReferenceTables() => _dumpGREFTableMethod.Invoke(null, emptyArgs);

        public static long CountInstancesOfClass(JavaClass javaClass, bool assignable)
        {
            var result = _countInstancesOfClassMethod.Invoke(null, new JavaObject[2] { javaClass, assignable});
            return (long)result;
        }

        public static IDisposable SetupGlobalRefDump()
        {
            var instance = new Dalvik();
            instance.SetupGlobalRefDump(TimeSpan.FromSeconds(3));
            return instance;
        }

        public void SetupGlobalRefDump(TimeSpan interval) =>
            Task.Run(() => DebugGlobalRefWorker(interval, _tokenSource.Token));

        private async Task DebugGlobalRefWorker(TimeSpan interval, CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                DumpReferenceTables();
                await Task.Delay(interval).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            _tokenSource.Cancel();
            _tokenSource.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
