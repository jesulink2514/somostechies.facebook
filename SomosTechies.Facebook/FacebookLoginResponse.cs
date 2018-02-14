namespace SomosTechies.Facebook
{
    public class FacebookLoginResponse
    {
        public FacebookLoginResponse(bool isCorrect,FacebookUser user,string message)
        {
            IsCorrect = isCorrect;
            User = user;
            Message = message;
        }
        public bool IsCorrect { get;}
        public FacebookUser User { get; }
        public string Message { get; }
    }
}