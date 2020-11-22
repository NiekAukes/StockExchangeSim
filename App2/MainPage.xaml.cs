using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Eco;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    
    public sealed partial class MainPage : Page
    {
        public MainPage inst = null;
        public Company cp = new Company();
        public List<Stock> stocks = new List<Stock>();
        public Exchange exchange = new Exchange();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Sub_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
