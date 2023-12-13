namespace Launcher.Model
{
    public interface ILoginModel
    {
        string Username { get; set; }
        Observable<string> UsernameProperty { get; }
        string Password { get; set; }
        Observable<string> PasswordProperty { get; }
        string Status { get; set; }
        Observable<string> StatusProperty { get; }
        string LoginToken { get; set; }
        Observable<string> LoginTokenProperty { get; }
    }
}
