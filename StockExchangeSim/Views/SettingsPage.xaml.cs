using Eco;
using StockExchangeSim.Helpers;
using StockExchangeSim.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace StockExchangeSim.Views
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/release/docs/UWP/pages/settings-codebehind.md
    // TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { Set(ref _elementTheme, value); }
        }

        private string _versionDescription;

        public string VersionDescription
        {
            get { return _versionDescription; }

            set { Set(ref _versionDescription, value); }
        }

        public SettingsPage()
        {
            InitializeComponent();


            CustomSeed.IsChecked = Master.fCustomSeed;
            asyncFieldFlag.IsChecked = Master.fAsyncFields;
            Seed.Text = Master.CustomSeed.ToString();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            VersionDescription = GetVersionDescription();
            await Task.CompletedTask;
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        private async void ThemeChanged_CheckedAsync(object sender, RoutedEventArgs e)
        {
            var param = (sender as RadioButton)?.CommandParameter;

            if (param != null)
            {
                await ThemeSelectorService.SetThemeAsync((ElementTheme)param);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private async void Seed_TextChanged(object sender, RoutedEventArgs e)
        {
            //parse naar getal
            Int32 seed = 0;
            try
            {
                seed = Int32.Parse(Seed.Text);
            }
            catch (Exception ex)
            {
                MessageDialog messageDialog = new MessageDialog("Invalid seed");
                messageDialog.Commands.Add(new UICommand("Close"));
                await messageDialog.ShowAsync();
                return;
            }
            Master.CustomSeed = seed;
        }

        private void CustomSeed_Click(object sender, RoutedEventArgs e) => Master.fCustomSeed = CustomSeed.IsChecked.Value;

        private void asyncCompaniesFlag_Click(object sender, RoutedEventArgs e) => Master.fAsyncCompanies = asyncCompaniesFlag.IsChecked.Value;

        private void asyncFieldFlag_Click(object sender, RoutedEventArgs e) => Master.fAsyncFields = asyncFieldFlag.IsChecked.Value;

        private void ChkbxLiqTarg_Click(object sender, RoutedEventArgs e) => Master.fCustomLiquidityTarget = ChkbxLiqTarg.IsChecked.Value;
        private async void customLiquidityTargetVal_LostFocus(object sender, RoutedEventArgs e)
        {
            int liqtarg = 0;
            if (Int32.TryParse(customLiquidityTargetVal.Text, out liqtarg))
            {
                Master.CustomLiqTarget = liqtarg;
                ErrorCustomLiqBox.Text = "";
                customLiquidityTargetVal.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                return;
            }
            ErrorCustomLiqBox.Text = "Input is not an int!";
            customLiquidityTargetVal.BorderBrush = new SolidColorBrush(Color.FromArgb(225, 0, 0, 255));
        }
    
    }
}
