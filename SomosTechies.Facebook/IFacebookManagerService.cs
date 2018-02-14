using System;
using System.Threading.Tasks;

namespace SomosTechies.Facebook
{
    public interface IFacebookManagerService
    {
        Task<FacebookLoginResponse> Login();

        void Logout();
    }
}
