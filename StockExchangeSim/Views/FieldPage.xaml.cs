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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace StockExchangeSim.Views
{
    public sealed partial class FieldPage : UserControl
    {
        public FieldPage()
        {

            this.InitializeComponent();

            this.DataContext = this;
        }
        private string _text = "TestField";
        public string fieldText
        {
            private set
            {
                _text = value;
               
            }
            get
            {
                return _text;
            }
        }

        #region AnimationStuffShit

        private void FieldBlock_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            TileRenderTransform.CenterX = Width / 2;
            TileRenderTransform.CenterY = Height / 2;

            //Tile.BorderBrush = new SolidColorBrush(color: Windows.UI.Color.FromArgb(255, 255, 255, 255));
            //Tile.BorderThickness = new Thickness(2);

            //DoubleAnimation animation = new DoubleAnimation();
            //animation.From = Tile.Scale.X;
            //animation.To = 1.2f;
            //animation.Duration = new Duration(TimeSpan.FromMilliseconds(350));
            //Tile.BeginAnimation(Scale, animation);
            revScaleAnim.Stop();
            scaleAnim.Begin();
        }
        private async void FieldBlock_PointerExited(object sender, PointerRoutedEventArgs e)
        {

            Tile.BorderBrush = new SolidColorBrush(color: Windows.UI.Color.FromArgb(0, 255, 255, 255));
            Tile.BorderThickness = new Thickness(2);

            /*Windows.UI.Composition.CompositionAnimation tileAnim = new Windows.UI.Composition.CompositionAnimation
            {
                Target = "Tile.Scale.X"
               
            };
            
            tileAnim.*/

            //TileRenderTransform.ScaleX = 1;
            //TileRenderTransform.ScaleY = 1;
            scaleAnim.Stop();
            revScaleAnim.Begin();
        }
        public float ScaleX
        {
            get { return Tile.Scale.X; }
        }
        public float ScaleY
        {
            get { return Tile.Scale.Y; }
        }

        private void FieldBlock_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            TileRenderTransform.CenterX = Width / 2;
            TileRenderTransform.CenterY = Height / 2;
            TileRenderTransform.ScaleX = 1.4f;
            TileRenderTransform.ScaleY = 1.4f;
        }

        private void FieldBlock_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            TileRenderTransform.CenterX = Width / 2;
            TileRenderTransform.CenterY = Height / 2;

            TileRenderTransform.ScaleX = TileRenderTransform.ScaleX - 0.2f;
            TileRenderTransform.ScaleY = TileRenderTransform.ScaleY - 0.2f;
        }
        #endregion
    }

}
