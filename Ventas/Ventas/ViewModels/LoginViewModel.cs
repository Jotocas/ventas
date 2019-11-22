namespace Ventas.ViewModels
{
    using Services;
    using GalaSoft.MvvmLight.Command;
    using Views;
    using ViewModels;
    using System.Windows.Input;
    using Xamarin.Forms;
    using Ventas.Helpers;

    public class LoginViewModel : BaseViewModel
    {
        #region Servicios
        private ApiService apiService;
        #endregion

        #region Atributos
        private string usuario;
        private string password;
        private bool isRunning;
        private bool isEnabled;
        #endregion

        #region Propiedades
        public bool IsEnabled
        {
            get { return isEnabled; }
            set { SetValue(ref isEnabled, value); }
        }
        public string Usuario
        {
            get
            {
                return usuario;
            }
            set
            {
                SetValue(ref usuario, value);
            }
        }
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                SetValue(ref password, value);
            }
        }
        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
            set
            {
                SetValue(ref isRunning, value);
            }
        }
        public bool IsRemembered { get; set; }
        #endregion

        #region Comandos
        public ICommand LoginCommand
        {
            get
            {
                return new RelayCommand(Login);
            }
        }

        private async void Login()
        {
            if (string.IsNullOrEmpty(this.Usuario))
            {
                await Application.Current.MainPage.DisplayAlert(
                      "Error",
                      "Debe de Ingresar su Email",
                      "Accept"
                      );
                return;
            }

            if (string.IsNullOrEmpty(this.Password))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debe de Ingresar su Password",
                    "Accept"
                    );
                return;
            }

            this.IsRunning = false;
            this.IsEnabled = true;
            var connection = await this.apiService.CheckConnection();

            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    connection.Message,
                    "Accept");
                return;
            }

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var token = await this.apiService.GetToken(url, this.usuario, this.password);

            if (token == null)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "El Token es nulo",
                    "Accept");
                return;
            }

            if (string.IsNullOrEmpty(token.AccessToken))
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    token.ErrorDescription,
                    "Accept");
                this.Password = string.Empty;
                return;
            }

            var mainViewModel = MainViewModel.GetInstance();
            Settings.TokenType = token.TokenType;
            Settings.AccessToken = token.AccessToken;
            Settings.IsRemenbered = this.IsRemembered;

            this.IsRunning = false;
            this.IsEnabled = true;

            this.Usuario = string.Empty;
            this.Password = string.Empty;

            mainViewModel.Products = new ProductsViewModel();
            Application.Current.MainPage = new ProductsPage();
        }
        #endregion

        #region Constructores
        public LoginViewModel()
        {
            this.apiService = new ApiService();
            this.IsRemembered = true;
            this.isEnabled = true;

            this.Usuario = "jtorresc73@gmail.com";
            this.Password = "123456";
        }
        #endregion
    }
}
