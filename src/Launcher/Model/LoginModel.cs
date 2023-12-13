using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Model
{
    public class LoginModel : ILoginModel
    {
        public Observable<string> UsernameProperty { get; }
        public string Username
        {
            get => UsernameProperty.Value;
            set => UsernameProperty.Value = value;
        }
        public Observable<string> PasswordProperty { get; }
        public string Password
        {
            get => PasswordProperty.Value;
            set => PasswordProperty.Value = value;
        }
        public Observable<string> StatusProperty { get; }
        public string Status
        {
            get => StatusProperty.Value;
            set => StatusProperty.Value = value;
        }
        public Observable<string> LoginTokenProperty { get; }
        public string LoginToken
        {
            get => LoginTokenProperty.Value;
            set => LoginTokenProperty.Value = value;
        }

        public LoginModel()
        {
            UsernameProperty = new Observable<string>("");
            PasswordProperty = new Observable<string>("");
            StatusProperty = new Observable<string>("");
            LoginTokenProperty = new Observable<string>(null);
        }
    }
}
