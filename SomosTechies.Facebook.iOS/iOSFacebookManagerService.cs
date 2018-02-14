using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Facebook.CoreKit;
using Facebook.LoginKit;
using Foundation;
using UIKit;

namespace SomosTechies.Facebook.iOS
{
    public class iOSFacebookManagerService : IFacebookManagerService
#pragma warning restore IDE1006 // Naming Styles
    {
        public Action<FacebookUser, string> _onLoginComplete;


        public void Login(Action<FacebookUser, string> onLoginComplete)
        {
            _onLoginComplete = onLoginComplete;
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }

            var tcs = new TaskCompletionSource<FacebookUser>();
            LoginManager manager = new LoginManager();
            manager.LogOut();
            manager.LoginBehavior = LoginBehavior.SystemAccount;
            manager.LogInWithReadPermissions(new string[] { "public_profile", "email" }, vc, (result, error) =>
            {
                if (error != null || result == null || result.IsCancelled)
                {
                    if (error != null)
                        _onLoginComplete?.Invoke(null, error.LocalizedDescription);
                    if (result.IsCancelled)
                        _onLoginComplete?.Invoke(null, "User Cancelled!");

                    tcs.TrySetResult(null);
                }
                else
                {
                    var request = new GraphRequest("me", new NSDictionary("fields", "id, first_name, email, last_name, picture.width(1000).height(1000)"));
                    request.Start((connection, result1, error1) =>
                    {
                        if (error1 != null || result1 == null)
                        {
                            Debug.WriteLine(error1.LocalizedDescription);
                            tcs.TrySetResult(null);
                        }
                        else
                        {
                            var id = string.Empty;
                            var firstName = string.Empty;
                            var email = string.Empty;
                            var lastName = string.Empty;
                            var url = string.Empty;

                            try
                            {
                                id = result1.ValueForKey(new NSString("id"))?.ToString();
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                            }

                            try
                            {
                                firstName = result1.ValueForKey(new NSString("first_name"))?.ToString();
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                            }

                            try
                            {
                                email = result1.ValueForKey(new NSString("email"))?.ToString();
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                            }

                            try
                            {
                                lastName = result1.ValueForKey(new NSString("last_name"))?.ToString();
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                            }

                            try
                            {
                                url = ((result1.ValueForKey(new NSString("picture")) as NSDictionary)?.ValueForKey(new NSString("data")) as NSDictionary)?.ValueForKey(new NSString("url")).ToString();
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                            }

                            tcs.TrySetResult(new FacebookUser(id, result.Token.TokenString, firstName, lastName, email, url));
                            _onLoginComplete?.Invoke(new FacebookUser(id, result.Token.TokenString, firstName, lastName, email, url), string.Empty);
                        }
                    });
                }
            });
        }

        private TaskCompletionSource<FacebookLoginResponse> _loginCompletionSource;
        public Task<FacebookLoginResponse> Login()
        {
            _loginCompletionSource = new TaskCompletionSource<FacebookLoginResponse>();
            
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }

           var manager = new LoginManager();
            manager.LogOut();
            manager.LoginBehavior = LoginBehavior.SystemAccount;
            manager.LogInWithReadPermissions(new string[] { "public_profile", "email" }, vc, (result, error) =>
            {
                if (error != null || result == null || result.IsCancelled)
                {
                    if (error != null)
                    {
                        var r = new FacebookLoginResponse(false,null,error.LocalizedDescription);
                        _loginCompletionSource.SetResult(r);
                    }

                    if (result.IsCancelled)
                    {
                        var r = new FacebookLoginResponse(false, null,"User cancelled login");
                        _loginCompletionSource.SetResult(r);
                    }
                }
                else
                {
                    var request = new GraphRequest("me", new NSDictionary("fields", "id, first_name, email, last_name, picture.width(1000).height(1000)"));
                    request.Start((connection, result1, error1) =>
                    {
                        if (error1 != null || result1 == null)
                        {
                            Debug.WriteLine(error1.LocalizedDescription);
                            var r = new FacebookLoginResponse(false,null,error1.LocalizedDescription);
                            _loginCompletionSource.SetResult(r);
                        }
                        else
                        {
                            var id = string.Empty;
                            var firstName = string.Empty;
                            var email = string.Empty;
                            var lastName = string.Empty;
                            var url = string.Empty;

                            try
                            {
                                id = result1.ValueForKey(new NSString("id"))?.ToString();
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                            }

                            try
                            {
                                firstName = result1.ValueForKey(new NSString("first_name"))?.ToString();
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                            }

                            try
                            {
                                email = result1.ValueForKey(new NSString("email"))?.ToString();
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                            }

                            try
                            {
                                lastName = result1.ValueForKey(new NSString("last_name"))?.ToString();
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                            }

                            try
                            {
                                url = ((result1.ValueForKey(new NSString("picture")) as NSDictionary)?.ValueForKey(new NSString("data")) as NSDictionary)?.ValueForKey(new NSString("url")).ToString();
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                            }

                            var user = new FacebookUser(id, result.Token.TokenString, firstName, lastName, email, url);
                            var r = new FacebookLoginResponse(true,user,string.Empty);
                            _loginCompletionSource.SetResult(r);
                        }
                    });
                }
            });

            return _loginCompletionSource.Task;
        }

        public void Logout()
        {
            LoginManager manager = new LoginManager();
            manager.LogOut();
        }
    }
}