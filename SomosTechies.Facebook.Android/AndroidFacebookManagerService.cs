using System;
using System.Collections.Generic;
using Android.App;
using Org.Json;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using SomosTechies.Facebook;
using System.Threading.Tasks;

namespace SomosTechies.Facebook.Android
{

    public class AndroidFacebookManagerService : Java.Lang.Object, IFacebookManagerService, IFacebookCallback, GraphRequest.IGraphJSONObjectCallback
    {
        private static Func<Activity> _currentActivityDelegate;
        public ICallbackManager CallbackManager;

        public static void Init(Func<Activity> currentActivityDelegate)
        {
            _currentActivityDelegate = currentActivityDelegate;
        }

        public AndroidFacebookManagerService()
        {
            CallbackManager = CallbackManagerFactory.Create();
            LoginManager.Instance.RegisterCallback(CallbackManager, this);
        }

        public void Logout()
        {
            LoginManager.Instance.LogOut();
        }

        #region IFacebookCallback
        public void OnSuccess(Java.Lang.Object result)
        {
            if (result is LoginResult n)
            {
                var request = GraphRequest.NewMeRequest(n.AccessToken, this);
                var bundle = new global::Android.OS.Bundle();
                bundle.PutString("fields", "id, first_name, email, last_name, picture.width(500).height(500)");
                request.Parameters = bundle;
                request.ExecuteAsync();
            }
            else
            {
                _loginTaskCompletionSource.SetCanceled();
            }
        }

        public void OnCancel()
        {
            _loginTaskCompletionSource.SetResult(new FacebookLoginResponse(false,null,"Canceled."));
        }

        public void OnError(FacebookException error)
        {
            var r = new FacebookLoginResponse(false,null,error?.Message);
            _loginTaskCompletionSource.SetResult(r);
        }
        public void OnCompleted(JSONObject p0, GraphResponse p1)
        {
            var id = string.Empty;
            var firstName = string.Empty;
            var email = string.Empty;
            var lastName = string.Empty;
            var pictureUrl = string.Empty;

            if (p0.Has("id"))
                id = p0.GetString("id");

            if (p0.Has("first_name"))
                firstName = p0.GetString("first_name");

            if (p0.Has("email"))
                email = p0.GetString("email");

            if (p0.Has("last_name"))
                lastName = p0.GetString("last_name");

            if (p0.Has("picture"))
            {
                var p2 = p0.GetJSONObject("picture");
                if (p2.Has("data"))
                {
                    var p3 = p2.GetJSONObject("data");
                    if (p3.Has("url"))
                    {
                        pictureUrl = p3.GetString("url");
                    }
                }
            }

            var user = new FacebookUser(id, AccessToken.CurrentAccessToken.Token, firstName, lastName, email,pictureUrl);
            var res = new FacebookLoginResponse(true,user,string.Empty);
            _loginTaskCompletionSource.SetResult(res);
        }

        private TaskCompletionSource<FacebookLoginResponse> _loginTaskCompletionSource;
        public Task<FacebookLoginResponse> Login()
        {
            _loginTaskCompletionSource = new TaskCompletionSource<FacebookLoginResponse>();

            LoginManager.Instance.SetLoginBehavior(LoginBehavior.NativeWithFallback);
            LoginManager.Instance.LogInWithReadPermissions(_currentActivityDelegate(), new List<string> { "public_profile", "email" });

            return _loginTaskCompletionSource.Task;
        }
        #endregion
    }
}