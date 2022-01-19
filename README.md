# Xamarin Playground

A test project to try out different aspects in Xamarin.Forms development.

Initial Xamarin.Forms Version: 5.0

## Secrets and app settings management 

1. Install [Mobile.BuildTool](https://github.com/dansiegel/Mobile.BuildTools) Nuget Package in code sharing project (the non-(iOS/Android) platform one).

![Mobile.BuildTool Nuget Package](doc/appSettings/mobilebuildtools-install.png)

2. After installing, `.gitignore` will have the following entries:

![.ignore modifications](doc/appSettings/mobilebuildtools-ignore.png)

and `buildtools.json` will be added to project solution root.

![buildtools.json added](doc/appSettings/mobilebuildtools-file-added.png)

3. add `appsettings.json` or `appsettings[.env].json` in non-sharing project root or solution root.

![buildtools.json added](doc/appSettings/mobilebuildtools-appsetting.png)

with content like this:

![buildtools.json added](doc/appSettings/mobilebuildtools-appsetting-content.png)

```json
{
  "key": "value"
}
```

4. Edit `buildtools.json`, add the following section at top level:

```json
"appSettings": {
  "DeclarativeSharp": [{
    "accessibility": "Public",
    "namespace": "MyNamespace",
    "className": "MySettings",
    "properties": [{
      "name": "GoogleMapKey",
      "type": "String"
    }]
  }]
},
```
Optional field name | Default value
:---|:--- 
accessibility | Internal
namespace | Helpers
classname | AppSettings
prefix | BuildTools_

**Note: the section name must be `appSettings` and it must have a property named the same as your project name (the one installed the nuget package). In this case i.e `DeclarativeSharp`** 

**When build in CI/CD pipeline, the tool will look for `[prefix]property_name` environment variable. In our example this will become `BuildTools_GoogleMapKey`.**

The `properties` array should specify what properties the generated class will have. It have to match the key-value pairs specified in following places:

from [MobileBuiltTools](https://mobilebuildtools.com/config/appsettings/):
```
- buildtools.json Environment Defaults
- buildtools.json Environment Configuration (i.e. Debug, Release)
- System Environment
- Recursively load legacy secrets.json from the Project directory to the Solution directory
- Recursively load appsettings.json from the Project directory to the Solution directory
```

The file should look like this afterwards:

![buildtools.json added](doc/appSettings/mobilebuildtools-config.png)

5. Clean the whole solution.
6. Build the shared code project (the one with MobileBuildTools Nuget package installed).
7. If everything setup correctly, a static class will be generated in the `obj` folder:

![buildtools.json added](doc/appSettings/mobilebuildtools-generated-class.png)

8. Rebuild the `Android` and `iOS` project.
9. You should able to use the app settings in platform project now.

iOS: `AppDelegate.cs`

![use appsetting in iOS](doc/appSettings/mobilebuildtools-ios.png)

Android: `MainActivity.cs`

![use appsetting in Android](doc/appSettings/mobilebuildtools-android.png)

**Note: If the generated class cannot be referenced in platform project or you have updated `buildtools.json`/`appsettings.json` for new setting item, repeat steps 5-9.**

## Declarative C# UI

Instead of using XAML for UI and layout, we can simply use single language (C#) to write UI/Layout code, as well as logic code. To boost productivity via better IDE and code refactoring support.

1. Install `Xamarin.CommunityToolkit` & `Xamarin.CommuityToolkit.Markup` nuget package. It contains the `DeclarativeCSharp` plus other useful packages.

![declarative csharp install](doc/declarativeCSharp/declarative-csharp-install.png)

2. The code of your app entry point become like this:

![declarative csharp app](doc/declarativeCSharp/declarative-csharp-app.png)
```c#
public partial class App : Application {
    public App() {
        MainPage = new MainPage();
    }
}
```

3. The code of your content pages should like this:

![declarative csharp page](doc/declarativeCSharp/declarative-csharp-page.png)
```c#
public partial class MainPage : ContentPage {
    public MainPage() {
        Content = new StackLayout() {
            Children = {
                new Map() {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    InitialCameraUpdate = CameraUpdateFactory.NewCameraPosition(
                        new CameraPosition(
                            new Position(22.410772, 113.980277), 13, 30, 60)),
                },
                new Label() {
                    Text = "Hello Declarative C# UI!",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                }
            }
        };
    }
}
```
4. Now you can code in pure C#. Any code that still use XAML with code behind C# will remains intact and continue working.

## Using Google Map instead of built-in map.

Reference: [Xamarin.Forms Map Initialization and Configuration](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/map/setup)

1. Install `Xamarin.Forms.GoogleMaps` nuget package.
2. Install `Xamarin.Build.Download` nuget package. This is required to enable platform specific project to download native google map code during build. 
3. You will create the `Xamarin.Forms.GoogleMaps.Map` component in your content page like this:
```c#
new Map() {
    VerticalOptions = LayoutOptions.FillAndExpand,
    InitialCameraUpdate = CameraUpdateFactory.NewCameraPosition(
        new CameraPosition(
            new Position(22.410772, 113.980277),13,30,60)),
}
```
### Google Map on iOS
4. On iOS project, initialize google map at `AppDelegate.cs`, `FinishedLaunching` method:
```c#
public override bool FinishedLaunching(UIApplication app, NSDictionary options) {
    global::Xamarin.Forms.Forms.Init();

    Xamarin.FormsGoogleMaps.Init(key); // Load the key from app setting

    LoadApplication(new App());

    return base.FinishedLaunching(app, options);
}
```
![google map in iOS](doc/googleMap/googlemap-map.png)

To enable `LocationService`, add the following keys to `info.plist`:
- NSLocationAlwaysUsageDescription
- NSLocationWhenInUseUsageDescription
- NSLocationAlwaysAndWhenInUseUsageDescription

The values are arbitrary informative messages.  

![enable location service in iOS](doc/googleMap/googlemap-location-service-info-plist.png)

```xml
<key>NSLocationAlwaysUsageDescription</key>
<string>Can we use your location at all times?</string>
<key>NSLocationWhenInUseUsageDescription</key>
<string>Can we use your location when your application is being used?</string>
<key>NSLocationAlwaysAndWhenInUseUsageDescription</key>
<string>Can we use your location at all times?</string>
```
### Google Map on Android
1. It requires configuring the `manifest` section in `buildtools.json` of `MobileBuildTools` nuget package.

```json
"manifests": {
"token": "$$",
"variablePrefix": "Manifest_",
"missingTokensAsErrors": true,
"disable": false
},
```
Note: When build in CI/CD pipeline, Mobile.BuildTools will look for `[prefix][secretName]` environment variable. In our case: `Manifest_GoogleMapKey` 

2. Add Google Map API Key
Add the following key into `AndroidManifest.xml`, under `application` tag:
```xml
<meta-data android:name="com.google.android.geo.API_KEY" android:value="$GoogleMapKey$" />
```

Note:
- We use `$GoogleMapKey$` here so Mobile.BuildTools can replace with values specified in `appsettings.json`, thus no key will commit to source control.
- It seems Android emulator does care the value of API key, as long as the API key tag exists, the map will show up correctly. (Not certain, but no real Android machine can test yet.)

3. Backward compatibility to lower API level (<23)

Add the following key into `AndroidManifest.xml`, under `application` tag:
    
```xml
<uses-library android:name="org.apache.http.legacy" android:required="false" />
```

4. Using Location Service 
To use location service, add the following keys into `AndroidManifest.xml`:
- `android.permission.ACCESS_COARSE_LOCATION`
- `android.permission.ACCESS_FINE_LOCATION`
```xml
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
```

The final `AndroidManifest.xml` should look like this:
```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android" android:versionCode="1" android:versionName="1.0" package="io.verdantsparks.DeclarativeSharp">
    <uses-sdk android:minSdkVersion="21" android:targetSdkVersion="30" />
    <application android:label="DeclarativeSharp.Android">
        <meta-data android:name="com.google.android.geo.API_KEY" android:value="$GoogleMapKey$" />
        <meta-data android:name="com.google.android.gms.version" android:value="@integer/google_play_services_version" />
        <uses-library android:name="org.apache.http.legacy" android:required="false" />
    </application>

    <uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
    <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
</manifest>
```

5. Edit `MainActivity.cs`

- Initialize `Xamarin.FormsGoogleMaps` **AFTER** `Xamarin.Forms.Forms.Init()`
```c#
Xamarin.Forms.Forms.Init(this, savedInstanceState);

Xamarin.FormsGoogleMaps.Init(this, savedInstanceState, new PlatformConfig());
```
- Request location permission on start:
```c#
protected override void OnStart()
{
    base.OnStart();

    if ((int)Build.VERSION.SdkInt >= 23)
    {
        if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) != Permission.Granted)
        {
            RequestPermissions(LocationPermissions, RequestLocationId);
        }
        else
        {
            // Permissions already granted - display a message.
        }
    }
}
```
- Handle permission request result:
```c#
public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
{
    if (requestCode == RequestLocationId)
    {
        if ((grantResults.Length == 1) && (grantResults[0] == (int)Permission.Granted)) {}
        // Permissions granted - display a message.
        else {}
        // Permissions denied - display a message.
    }
    else
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
}
```


### Enable Google Map on Android Emulator
[Maps SDK for Android Quickstart](https://developers.google.com/maps/documentation/android-sdk/start)

**Note: By default Android Emulator cannot display Google Map in Apps or even their genuine GoogleMaps App.**

1. Use API level 28+ image with Google Play enabled.
![Android image with Google Play](doc/googleMap/googlemap-android-emulator-image.png)


2. Start emulator, go to [Setting] -> [Google Play] -> [Update], this will open Google Play App.


3. The trickiest part, you must click the menu on top right and click [Updates] to update the default Google Apps (e.g. GoogleMaps). **Don't get faked from the "Loading..." text, or clicking "sign-in" on Play app. It will just load and displaying "checking" forever!**  

![Android update Google Play](doc/googleMap/googlemap-update-play.png)

5. Your app should and Google Map App should display correctly.

![Google Map shown in Android App](doc/googleMap/googlemap-android-success.png)


## MVVM Architecture Data Binding
- Simple data binding example: `BindingTestPage.cs`

### Create ViewModel
```c#
public class BindingTestViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private int _sliderValue;
    public int SliderValue {
        get => _sliderValue;
        set {
            if (_sliderValue == value) return;

            _sliderValue = value;
            OnPropertyChanged(nameof(SliderValue));
        }
    }

    public BindingTestViewModel() {
        SliderValue = 0;
    }
}
```
1. A view model needs to implement `INotifyPropertyChanged` interface to enable control being notified.
2. For any property in view model, create private backing field and modify the setter, in setter `OnPropertyChanged` must be called, so the controls receive update notification.
3. The default implementation of `OnPropertyChanged` is sufficient and no need to change.

### Setup ContentPage UI
```c#
public BindingTestPage() {
    // IMPORTANT!
    BindingContext = vm = new BindingTestViewModel();

    Content = new StackLayout() {
        Children = {
            new Label() {
                    Text = "Should replace me.",
                    // You can override BindingContext here!
                    // BindingContext = anotherViewModel,
                }
                .Bind(Label.TextProperty, nameof(vm.SliderValue)
                    // Override here takes highest precedence.
                    // source: yetAnotherViewModel
                    ),
            MySlider,
            new Button() { Text = "Add"}.Invoke(button => button.Clicked += (_, _) => {
                vm.SliderValue += 1;
            }),
        },
    };
}
```
1. Create view model instance. Setting `BindingContext` of content page will inherit by all controls in visual tree. So you don't need to specify `BindingContext` for each control.
```c#
BindingContext = vm = new BindingTestViewModel();
```
2. You can override the page `BindingContext` for specific control, set it in controls initializer.
```c#
new Label() { BindingContext = anotherViewModel }
```
3. You can also override the page `BindingContext` in `.Bind()` method, setting the `source` parameter.
```c#
new Label() {}.Bind(Label.TextProperty, nameof(vm.SliderValue), source: yetAnotherViewModel),
```
4. Update view model with bind
```c#
public Slider MySlider =>
    new Slider() { Maximum = 100, }
        .Bind(Slider.ValueProperty, nameof(vm.SliderValue), BindingMode.TwoWay);
```
5. The complete page will have label text update as slider move.
![Data binding sample](doc/dataBinding/databinding-basic-sample.png)

## Entity Framework with SQLite

Xamarin use Mono, and Mono only support up to .netstandard2.1, thus Xamarin project cannot use `Microsoft.EntityFrameworkCore.Sqlite` version `6.0.0`, the last installable version is `5.0.12`.

`Xamarin.Essentials` nuget package is required for on device `FileSystem` object access.


## In-App Purchase (IAP) - iOS
