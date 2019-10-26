namespace Ventas.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using System.Windows.Input;
    using Ventas.Views;
    using Xamarin.Forms;
    public class MainViewModel
    {
        #region Propiedades
        public ProductsViewModel Products { get; set; }
        public AddProductViewModel AddProducts { get; set; }
       // public EditProductViewModel EditProduct { get; set; }

        #endregion

        #region Constructores
        public MainViewModel()
        {
            instance = this;
            this.Products = new ProductsViewModel();
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