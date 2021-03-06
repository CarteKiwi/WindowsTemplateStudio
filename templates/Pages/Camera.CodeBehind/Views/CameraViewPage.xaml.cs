using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Param_ItemNamespace.Views
{
    public sealed partial class CameraViewPage : Page, System.ComponentModel.INotifyPropertyChanged
    {
        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { Set(ref _errorMessage, value); }
        }

        public CameraViewPage()
        {
            InitializeComponent();
        }

        private async Task InitializeAsync()
        {
            try
            {
                await Camera.InitializeAsync();
            }
            catch (UnauthorizedAccessException)
            {
                ErrorMessage = "The app was denied access to the camera.";
            }
            catch (NotSupportedException)
            {
                ErrorMessage = "No video capture devices found.";
            }
        }

        private void Cleanup()
        {
            Camera.Cleanup();
        }

        private async void Photo_Click(object sender, RoutedEventArgs e)
        {
            Photo.Source = new BitmapImage(new Uri(await Camera.TakePhotoAsync()));
        }

        private void SwitchCamera_Click(object sender, RoutedEventArgs e)
        {
            Camera.Panel = Camera.Panel == Windows.Devices.Enumeration.Panel.Front ? Windows.Devices.Enumeration.Panel.Back : Windows.Devices.Enumeration.Panel.Front;
        }
    }
}