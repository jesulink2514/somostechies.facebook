using System;

namespace SomosTechies.Facebook
{
    public interface IFacebookManagerService
    {
        void Login(Action<FacebookUser, string> onLoginComplete);

        void Logout();
    }
}
