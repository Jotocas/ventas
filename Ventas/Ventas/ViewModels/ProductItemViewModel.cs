

namespace Ventas.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using System;
    using System.Windows.Input;
    using Ventas.Common.Models;
    using Services;
    using Xamarin.Forms;
    using System.Linq;
    using Ventas.Views;

    public class ProductItemViewModel:Product
    {
        #region Atributos
        private ApiService apiService;
        #endregion


        #region Constructores
        public ProductItemViewModel()
        {
            this.apiService = new ApiService();      
        }
        #endregion

        #region Comandos
        //public ICommand EditProductCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(EditProduct);
        //    }
        //}

        //private async void EditProduct()
        //{
        //    // MainViewModel.GetInstance().EditProduct = new EditProductViewModel(this);
        //    // await Application.Current.MainPage.Navigation.PushAsync(new EditProductPage());
        //}

        public ICommand DeleteProductCommand
        {
            get
            {
                return new RelayCommand(DeleteProduct);
            }
        }

        private async void DeleteProduct()
        {

            var answer = await Application.Current.MainPage.DisplayAlert(
                "Confirmar",
                "¿Desea Eliminar este Registro",
                "Si",
                "No"
                );

            if (!answer)
            {
                return;
            }

            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", connection.Message, "Aceptar");
                return;
            }

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();

            var response = await this.apiService.Delete(url, prefix, controller, this.ProductId);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "Aceptar");
                return;
            }

            var productViewModel = ProductsViewModel.GetInstance();

            var deleteProduct = productViewModel.Products.Where(
               p => p.ProductId == this.ProductId
                ).FirstOrDefault();

            if (deleteProduct!=null)
            {
                productViewModel.Products.Remove(deleteProduct);
            }

        }

        #endregion
    }
}
