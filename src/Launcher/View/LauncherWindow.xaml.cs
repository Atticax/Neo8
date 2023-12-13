using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Input;
//using S4Launcher.LoginAPI;
using System.Text.RegularExpressions;
using System.Reflection;
using Launcher.Model;
using System.Windows.Data;
using System.Windows.Controls;

namespace Launcher.View
{
    public partial class LauncherWindow : Window, ILauncherWindow
    {
        private readonly Application _app;
        private readonly ILoginModel _loginModel;


        public LauncherWindow(Application app, ILoginModel loginModel)
        {
            _app = app;
            _loginModel = loginModel;
            //Constants.LoginWindow = this;
            InitializeComponent();
            RegisterEventHandlers();
            BindData();
        }

        private void BindData()
        {
            _loginModel.UsernameProperty.BidirectionalBind(textFieldLoginUsername, TextBox.TextProperty);
            _loginModel.PasswordProperty.BidirectionalBind(
                x => passwordFieldLoginPassword.PasswordChanged += (s, e) => x(passwordFieldLoginPassword.Password),
                x => passwordFieldLoginPassword.Password = x
            );
            _loginModel.StatusProperty.Bind(lableStatus, Label.ContentProperty);
        }

        #region EventHandlers
        private void RegisterEventHandlers()
        {
            buttonExit.MouseLeftButtonDown += (s, e) => _app.Shutdown();
            MouseLeftButtonDown += (s, e) => DragMove();
            passwordFieldLoginPassword.PasswordChanged += (s, e) => PasswordPlaceholderControl();
            passwordFieldLoginPassword.IsKeyboardFocusedChanged += (s, e) => PasswordPlaceholderControl();
        }
        private void PasswordPlaceholderControl()
        {
            if (passwordFieldLoginPassword.Password == string.Empty && !passwordFieldLoginPassword.IsKeyboardFocused)
            {
                passwordPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                passwordPlaceholder.Visibility = Visibility.Collapsed;
            }

        }
        #endregion

        public void AttachLoginButtonActionHandler(MouseButtonEventHandler handler)
        {
            buttonLogin.MouseLeftButtonDown += handler;
        }

        /*public void UpdateLabel(string message)
        {
            Dispatcher.Invoke(() => { result_label.Content = message; });

        }
        public string GetUsername()
        {
            return Dispatcher.Invoke(() => { return textFieldLoginUsername.Text; });
        }
        public string GetPassword()
        {
            return Dispatcher.Invoke(() => { return passwordFieldLoginPassword.Password; });
        }
        public void Error(string message)
        {
            MessageBox.Show(message);
            Environment.Exit(0);
        }

        public void LoginError(string error)
        {
            Dispatcher.Invoke(() =>
            {

                passwordFieldLoginPassword.IsEnabled = false;
                textFieldLoginUsername.IsEnabled = false;
                //btn_Login.IsEnabled = false;
                //btn_Login.Visibility = Visibility.Hidden;
                result_label.Content = error;

            });
        }

        /*private void Login_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
                if (passwordFieldLogin.Password.Length > 4 || login_username.Text.Length > 4)
                {
                    if (new Regex("^[a-zA-Z0-9.*_-]*$").IsMatch(login_username.Text))
                    {
                        //LoginClient.Connect(Constants.ConnectEndPoint);
                    }
                    else
                    {
                        result_label.Content = "Username Contains Invalid Characters";
                    }
                }
                else
                {
                    result_label.Content = "Id or Pw is less than 4 characters";
                }

            
        }
        public void Start(string code)
        {
            Dispatcher.Invoke(() =>
            {
                
                    try
                    {
                        Process.Start("s4client.exe", string.Format("-rc:eu -lac:{2} -auth_server_ip:{0} -aeria_acc_code:{1}", /*(object)Constants.ConnectEndPoint.Address*(object)code, (object)code, "eng"));
                        Environment.Exit(0);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Failed to run S4Client.exe", "Error");
                        Environment.Exit(0);
                    }


            });
        }*/

    }

}
