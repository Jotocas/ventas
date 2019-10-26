namespace Ventas.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Services;
    using Ventas.Common.Models;
    using Xamarin.Forms;

    public class ProductsViewModel : BaseViewModel
    {
        #region Atributos
        private ApiService apiService;
        private bool isRefreshing;
        private ObservableCollection<ProductItemViewModel> products;

        #endregion

        #region Propiedades
        public List<Product> MyProducts { get; set; }
        public ObservableCollection<ProductItemViewModel> Products
        {
            get { return this.products; }
            set { this.SetValue(ref this.products, value); }
        }

        public bool IsRefreshing
        {
            get { return this.isRefreshing; }
            set { this.SetValue(ref this.isRefreshing, value); }
        }
        #endregion

        #region Constructores
        public ProductsViewModel()
        {
            instance = this;
            this.apiService = new ApiService();
            this.LoadProducts();
        }
        #endregion

        #region Metodos
        private async void LoadProducts()
        {
            this.IsRefreshing = true;

            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.IsRefreshing = false;
                //await Application.Current.MainPage.DisplayAlert("Error", connection.Message, "Aceptar");
                // await Application.Current.MainPage.DisplayAlert(Languages.Error, connection.Message, Languages.Accept);
                return;
            }

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();

            var response = await this.apiService.GetList<Product>(url, prefix, controller);
            if (!response.IsSuccess)
            {

                //  await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "Aceptar");
                this.IsRefreshing = false;
                return;
            }

            this.IsRefreshing = false;
           this.MyProducts = (List<Product>)response.Result;

            this.RefreshList();
            }

        public void RefreshList()
        {
            var myListProductItemViewModel = this.MyProducts.Select(prop => new ProductItemViewModel
            {
                Description = prop.Description,
                ImageArray = prop.ImageArray,
                ImagePath = prop.ImagePath,
                IsAvailable = prop.IsAvailable,
                Price = prop.Price,
                ProductId = prop.ProductId,
                PublishOn = prop.PublishOn,
                Remarks = prop.Remarks

            });

            this.Products = new ObservableCollection<ProductItemViewModel>(myListProductItemViewModel.OrderBy(p => p.Description));

        }
        #endregion

        #region Comandos

        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(LoadProducts);
            }
        }
        #endregion

        #region Singleton
        private static ProductsViewModel instance;
        public static ProductsViewModel GetInstance()
        {
            if (instance==null)
            {
                return new ProductsViewModel();
            }

            return instance;
        }
        #endregion
    }
}