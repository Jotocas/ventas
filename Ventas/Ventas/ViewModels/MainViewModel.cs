namespace Ventas.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using Ventas.Views;
    using Xamarin.Forms;

    public class MainViewModel
    {
        #region Propiedades
        public ObservableCollection<MenuItemViewModel> Menu { get; set; }
        public ProductsViewModel Products { get; set; }
        public LoginViewModel Login { get; set; }
        public AddProductViewModel AddProducts { get; set; }
        public EditProductViewModel EditProduct { get; set; }

        #endregion

        #region Constructores
        public MainViewModel()
        {
            instance = this;
            this.Login = new LoginViewModel();
            this.LoadMenu();
        }
        #endregion

        #region Metodos
        private void LoadMenu()
        {
            this.Menu = new ObservableCollection<MenuItemViewModel>();

            this.Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_settings.png",
                PageName = "AboutPage",
                Title = "Acerca de.."//Languages.MyProfile,
            });

            this.Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_bar_chart.png",
                PageName = "StaticsPage",
                Title = "Configuración"//Languages.Statics,
            });

            this.Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_exit_to_app.png",
                PageName = "LoginPage",
                Title = "Salir"//Languages.LogOut,
            });
        }
        #endregion

        #region Comandos

        public ICommand AddProductCommand
        {
            get { return new RelayCommand(GoToAddProduct); }

        }

        private async void GoToAddProduct()
        {
            this.AddProducts = new AddProductViewModel();
            await Application.Current.MainPage.Navigation.PushAsync(new AddProductPage());
        }
        #endregion

        #region Singleton
        private static MainViewModel instance;
        public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
                return new MainViewModel();
            }

            return instance;
        }
        #endregion
    }


}