# Xamarin.Management.Tools
 Tools to track and manage your Xamarin applications


 [![NuGet](https://img.shields.io/nuget/v/Xamarin.Management.Tools.svg)](https://www.nuget.org/packages/Xamarin.Management.Tools/)

## Getting Started

- Install the Xamarin.Management.Tools package

 ```
 Install-Package Xamarin.Management.Tools -Version 1.0.1
 ```

## SyncContextTracker
Track your application `SynchroniationContext` to detect deadlocks and potential deadlocks

- For Xamarin.Forms application
```C#
    public partial class App {

        protected override void OnStart() =>
            SyncContextTracker.Start(SyncContextTrackerMode.ReportPotentialDeadlocks);

        protected override void OnSleep() {
        }

        protected override void OnResume() {
        }
    }
```

- For Xamarin.Android (without Xamarin.Forms)
```C#
    public class MainActivity : AppCompatActivity {
        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            SyncContextTracker.Start(SyncContextTrackerMode.ReportPotentialDeadlocks);
        }
    }
```

- For Xamarin.IOS (without Xamarin.Forms)
```C#
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options) {
           SyncContextTracker.Start(SyncContextTrackerMode.ReportPotentialDeadlocks);
           base.FinishedLaunching(app, options);
        }
    }
```

## Dalvik VM (Android)

Track the GRefs on your Android application

```C#
    [Application]
    public class MainApplication : Application {
        protected MainApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer) { }

        public override void OnCreate() {
            base.OnCreate();

            Dalvik.SetupGlobalRefDump();
        }
    }
```

This will provide a log every 3 seconds, you can also configure a custom interval

```C#
    var vm = Dalvik();
    Dalvik.SetupGlobalRefDump(TimeSpan.FromSeconds(5));
    ....

    //Don't forget to discard it
    vm.Dispose();
```
