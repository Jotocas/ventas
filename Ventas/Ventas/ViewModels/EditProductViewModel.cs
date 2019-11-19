namespace Ventas.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using System.Windows.Input;
    using Xamarin.Forms;
    using Services;
    using Common.Models;
    using Plugin.Media;
    using Plugin.Media.Abstractions;
    using System.Linq;
    using System;

    public class EditProductViewModel : BaseViewModel
    {
        #region Atributos
        private Product product;
        private bool isRunning;
        private bool isEnabled;
        private ApiService apiService;
        private ImageSource imageSource;
        private MediaFile file;
        #endregion

        #region Propiedades
        public Product Product { get => product; set { this.SetValue(ref this.product, value); } }
        public bool IsRunning { get => isRunning; set => this.SetValue(ref isRunning, value); }
        public bool IsEnabled { get => isEnabled; set => this.SetValue(ref isEnabled, value); }
        public ImageSource ImageSource { get => imageSource; set => this.SetValue(ref imageSource, value); }
        #endregion

        #region Contructores
        public EditProductViewModel(Product product)
        {
            this.product = product;
            this.IsEnabled = true;
            this.apiService = new ApiService();
            this.ImageSource = product.ImageFullPath;
        }
        #endregion

        #region Comandos

        public ICommand ChangeImageCommand
        {
            get
            {
                return new RelayCommand(ChangeImage);
            }
        }
        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(Save);
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return new RelayCommand(Delete);
            }
        }

        private async void Delete()
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

            this.IsRunning = true;
            this.IsEnabled = false;

            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert("Error", connection.Message, "Aceptar");
                return;
            }

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();

            var response = await this.apiService.Delete(url, prefix, controller, this.Product.ProductId);
            if (!response.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "Aceptar");
                return;
            }

            var productViewModel = ProductsViewModel.GetInstance();

            var deleteProduct = productViewModel.MyProducts.Where(
               p => p.ProductId == this.Product.ProductId
                ).FirstOrDefault();

            if (deleteProduct != null)
            {
                productViewModel.MyProducts.Remove(deleteProduct);
            }
            productViewModel.RefreshList();

            this.IsRunning = false;
            this.IsEnabled = true;

            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async void ChangeImage()
        {
            await CrossMedia.Current.Initialize();

            var source = await Application.Current.MainPage.DisplayActionSheet(
                "¿De dónde vas a tomar la foto",
                "cancel",
                null,
                "De la galeria",
               "Tomar una nueva imagen"
                );

            if (source == "cancel")
            {
                this.file = null;
                return;
            }
            if (source == "Tomar una nueva imagen")
            {
                this.file = await CrossMedia.Current.TakePhotoAsync(
                    new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = "test.jpg",
                        PhotoSize = PhotoSize.Small,
                    }
                    );
            }
            else
            {
                this.file = await CrossMedia.Current.PickPhotoAsync();
            }

            if (this.file != null)
            {
                this.ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = this.file.GetStream();
                    return stream;
                }
                    );
            }
        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(this.Product.Description))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Es Obligatorio Ingresar la Descripcion",
                    "Accept"
                      );
                return;
            }


            if (this.Product.Price < 0)
            {
                await Application.Current.MainPage.DisplayAlert(
                   "Error",
                   "El precio debe ser mayor a 0",
                   "Accept"
                     );
                return;
            }
            this.IsRunning = true;
            this.IsEnabled = false;

            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert("Error", connection.Message, "Aceptar");
                return;
            }

            byte[] imageArray = null;
            if (this.file != null)
            {
                imageArray = FilesHelper.ReadFully(this.file.GetStream());
                this.Product.ImageArray = imageArray;
            }

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();

            var response = await this.apiService.Put(url, prefix, controller, this.Product, this.Product.ProductId);

            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "Aceptar");
                return;
            }

            var newProduct = (Product)response.Result;
            var productsViewModel = ProductsViewModel.GetInstance();

            var oldProduct = productsViewModel.MyProducts.Where(p => p.ProductId == this.Product.ProductId).FirstOrDefault();
            if (oldProduct != null)
            {
                productsViewModel.MyProducts.Remove(oldProduct);
            }


            productsViewModel.MyProducts.Add(newProduct);
            productsViewModel.RefreshList();

            this.IsRunning = false;
            this.IsEnabled = true;

            await Application.Current.MainPage.Navigation.PopAsync();

        }
        #endregion
    }
}
