using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using SomosTechies.Facebook;
using SomosTechies.Facebook.Android;
using Xamarin.Forms;

namespace DemoFbLogin.Droid
{
    [Activity(Label = "DemoFbLogin", Icon = "@drawable/icon", Theme = "@style/MainTheme", 
        MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            
            global::Xamarin.Forms.Forms.Init(this, bundle);
            AndroidFacebookManagerService.Init(() => this);
            DependencyService.Register<AndroidFacebookManagerService>();

            LoadApplication(new App());
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            var manager = DependencyService.Get<IFacebookManagerService>();
            (manager as AndroidFacebookManagerService)?.CallbackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }
    }
}

