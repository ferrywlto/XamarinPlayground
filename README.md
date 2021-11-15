# Xamarin Playground

A test project to try out different aspects in Xamarin development.

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

**Note: the section name must be `appSettings` and it must have a property named the same as your project name (the one installed the nuget package). In this case i.e `DeclarativeSharp`** 

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

1. Install `Xamarin.CommunityToolkit` nuget package. It contains the `DeclarativeCSharp` plus other useful packages.

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
