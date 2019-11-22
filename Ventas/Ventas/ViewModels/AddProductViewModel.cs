
namespace Ventas.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using System.Windows.Input;
    using Xamarin.Forms;
    using Services;
    using Ventas.Common.Models;
    using Plugin.Media;
    using Plugin.Media.Abstractions;
    using Ventas.Helpers;

    public class AddProductViewModel : BaseViewModel
    {

        #region Atributos
        private bool isRunning;
        private bool isEnabled;
        private ApiService apiService;
        private ImageSource imageSource;
        private MediaFile file;
        #endregion

        #region Propiedades

        public string Descripcion { get; set; }
        public string Price { get; set; }
        public string Remarks { get; set; }
        public bool IsRunning { get => isRunning; set => this.SetValue(ref isRunning, value); }
        public bool IsEnabled { get => isEnabled; set => this.SetValue(ref isEnabled, value); }
        public ImageSource ImageSource { get => imageSource; set => this.SetValue(ref imageSource, value); }
        #endregion

        #region Constructores
        public AddProductViewModel()
        {
            this.IsEnabled = true;
            this.apiService = new ApiService();
            this.ImageSource = "noproduct";
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
            if (string.IsNullOrEmpty(this.Descripcion))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Es Obligatorio Ingresar la Descripcion",
                    "Accept"
                      );
                return;
            }

            if (string.IsNullOrEmpty(this.Price))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Es Obligatorio Ingresar el Precio",
                    "Accept"
                      );
                return;
            }

            var price = decimal.Parse(this.Price);
            if (price < 0)
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
                // await Application.Current.MainPage.DisplayAlert(Languages.Error, connection.Message, Languages.Accept);
                return;
            }

            byte[] imageArray = null;
            if (this.file!=null)
            {
                imageArray = FilesHelper.ReadFully(this.file.GetStream());
            }

            var product = new Product
            {
                Description = this.Descripcion,
                Price = price,
                Remarks = this.Remarks,
                ImageArray=imageArray,
            };

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();

            var response = await this.apiService.Post(url, prefix, controller, product, Settings.TokenType, Settings.AccessToken);

            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "Aceptar");
                return;
            }

            var newProduct = (Product)response.Result;
            var productsViewModel = ProductsViewModel.GetInstance();
            productsViewModel.MyProducts.Add(newProduct);
            productsViewModel.RefreshList();

            this.IsRunning = false;
            this.IsEnabled = true;

            await Application.Current.MainPage.Navigation.PopAsync();

        }
        #endregion


    }
}
