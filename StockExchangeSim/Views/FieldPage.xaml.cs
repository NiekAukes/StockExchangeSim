using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
using Windows.UI.Xaml.Resources;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace StockExchangeSim.Views
{
    public sealed partial class FieldPage : UserControl
    {
        public FieldPage()
        {

            this.InitializeComponent();
            

            Tile.BorderThickness = new Thickness(TileBorderThickness);
            Tile.BorderBrush = TileNOTSelectedBrush;

            this.DataContext = this;
        }
        public double TileBorderThickness = 5;
        public double TileCornerRadius = 4;
        //public SolidColorBrush TileSelectedBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(220, 255, 255, 255));
        //public SolidColorBrush TileNOTSelectedBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 255, 255, 255));
        private SolidColorBrush tileselectedbrush;

        public SolidColorBrush TileSelectedBrush
        {
            get
            {
                return new SolidColorBrush(Windows.UI.Color.FromArgb(225, 230, 230, 230));
                
                    /* if (RequestedTheme == ElementTheme.Dark) {
                    tileselectedbrush = new SolidColorBrush(Windows.UI.Color.FromArgb(225, 225, 225, 225));
                }
                else {
                    tileselectedbrush = new SolidColorBrush(Windows.UI.Color.FromArgb(225, 21, 21, 21));
                }
                return tileselectedbrush;*/
            }
        }
        public SolidColorBrush TileNOTSelectedBrush
        {
            get
            {
                return new SolidColorBrush(Windows.UI.Color.FromArgb(0, 225, 225, 225));
            }
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
        private double tileHeight = 250;
        private double tileWidth  = 250;
        public double TileHeight
        {
            get { return tileHeight; }
            set { tileHeight = value;}
        }
        public double TileWidth
        {
            get { return tileWidth; }
            set { tileWidth = value;}
        }


        #region AnimationStuffShit

        public static Storyboard scaleUpStoryBoard = new Storyboard();
        public static Storyboard SecondscaleUpStoryBoard;

        public bool isMouseOver;
        //IMPORTANT BOOLEAN FOR CHECKING IF MOUSE IS OVER THE TILE, is used for scale animations (see Vector3 MouseOver and PointerExited)
        private void FieldBlock_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            isMouseOver = true;
            Tile.BorderThickness = new Thickness(TileBorderThickness);

            //Tile.BorderBrush = new SolidColorBrush(color: Windows.UI.Color.FromArgb(255, 255, 255, 255));
            //Tile.BorderThickness = new Thickness(2);

            //ColorAnimation bruh = AnimateBorderbrushProperty(Tile, new System.Numerics.Vector4(0, 255, 255, 255), new System.Numerics.Vector4(255, 255, 255, 255), 250);
            //Storyboard brushBoard = new Storyboard();
            //DoubleAnimation animateOpacityOfBorderBrush = CreateDoubleAnimation(backgroundbrush, 0, 1, "Opacity", 250);
           // brushBoard.Children.Add(animateOpacityOfBorderBrush); brushBoard.Begin();
            
            SizeUpFrameWorkElement(Tile, Tile.Scale.X, Tile.Scale.Y, 1.2f,1.2f, 125); //time in ms
        }

        private void FieldBlock_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            isMouseOver = false;
            //Tile.BorderBrush = new SolidColorBrush(color: Windows.UI.Color.FromArgb(0, 255, 255, 255));
            //Tile.BorderThickness = new Thickness(2);
            //ColorAnimation bruh2 = AnimateBorderbrushProperty(Tile, new System.Numerics.Vector4(255, 255, 255, 255), new System.Numerics.Vector4(0, 255, 255, 255), 150);
            //Storyboard brushBoard2 = new Storyboard();
            //DoubleAnimation opacityAnimBorderBrush = CreateDoubleAnimation(backgroundbrush, 255, 0, "Opacity", 150);
            //brushBoard2.Children.Add(opacityAnimBorderBrush);
            //brushBoard2.Begin();

            GridOfTile.BorderBrush = TileNOTSelectedBrush;
            GridOfTile.BorderThickness = new Thickness(TileBorderThickness);
            GridOfTile.CornerRadius = new CornerRadius(TileCornerRadius);

            SizeUpFrameWorkElement(Tile, 1.2f, 1.2f, 1.0f, 1.0f, 150); //time in ms
        }
        private void FieldBlock_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            //when mouse is pressed over element
            GridOfTile.BorderBrush = TileSelectedBrush;
            GridOfTile.BorderThickness = new Thickness(TileBorderThickness);
            GridOfTile.CornerRadius = new CornerRadius(TileCornerRadius);
        }

        private void FieldBlock_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            //when mouse is released over element
            GridOfTile.BorderBrush = TileNOTSelectedBrush;
            GridOfTile.BorderThickness = new Thickness(TileBorderThickness);
            GridOfTile.CornerRadius = new CornerRadius(TileCornerRadius);
        }
        private void GridOfTile_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            //is not used

        }

        public float ScaleX
        {
            get { return Tile.Scale.X; }
        }
        public float ScaleY
        {
            get { return Tile.Scale.Y; }
        }
        private System.Numerics.Vector3 scaleXYZ = new System.Numerics.Vector3(1.0f, 1.0f, 1.0f);
        public System.Numerics.Vector3 MouseOver
        {
            get{return isMouseOver ? new System.Numerics.Vector3(1.2f, 1.2f, 1.2f) : new System.Numerics.Vector3(1.0f, 1.0f, 1.0f);}
        }
        
        
        public static void SizeUpFrameWorkElement(FrameworkElement grid, double fromX, double fromY, double scaleX, double scaleY, Double mSecTimeSpan)
        {
            grid.RenderTransform = new ScaleTransform { ScaleX = fromX, ScaleY = fromY };
            DoubleAnimation animateScaleX = CreateDoubleAnimation(grid.RenderTransform, fromX, scaleX, "(ScaleTransform.ScaleX)", mSecTimeSpan);
            DoubleAnimation animateScaleY = CreateDoubleAnimation(grid.RenderTransform, fromY, scaleY, "(ScaleTransform.ScaleY)", mSecTimeSpan);

            if (scaleUpStoryBoard.GetCurrentState() == ClockState.Active || scaleUpStoryBoard.GetCurrentState() == ClockState.Filling)
            {
                SecondscaleUpStoryBoard = new Storyboard();
                SecondscaleUpStoryBoard.Children.Add(animateScaleX);
                SecondscaleUpStoryBoard.Children.Add(animateScaleY);
                SecondscaleUpStoryBoard.Begin();
            }
            else
            {                
                scaleUpStoryBoard.Children.Add(animateScaleX);
                scaleUpStoryBoard.Children.Add(animateScaleY);
                scaleUpStoryBoard.Begin();
            }
        }
        public static ColorAnimation AnimateBorderbrushProperty(FrameworkElement elem, System.Numerics.Vector4 from, System.Numerics.Vector4 to, double mSecTimeSpan)
        {
            
            ColorAnimation borderBrushAnim = new ColorAnimation();
            Storyboard.SetTarget(borderBrushAnim, elem);
            Storyboard.SetTargetProperty(borderBrushAnim, (new TargetPropertyPath(BorderBrushProperty)).ToString()); //BorderBrushProperty
            borderBrushAnim.From =  Windows.UI.Color.FromArgb((byte)from.W, (byte)from.X, (byte)from.Y, (byte)from.Z);
            borderBrushAnim.To =    Windows.UI.Color.FromArgb((byte)to.W,   (byte)to.X,   (byte)to.Y,   (byte)to.Z);
            borderBrushAnim.Duration = TimeSpan.FromMilliseconds(mSecTimeSpan);
            return borderBrushAnim;
        }

        private static DoubleAnimation CreateDoubleAnimation(DependencyObject frameworkElement, double fromX, double toX, string propertyToAnimate, Double mSecInterval)
        {
            DoubleAnimation animation = new DoubleAnimation();
            Storyboard.SetTarget(animation, frameworkElement);
            Storyboard.SetTargetProperty(animation, propertyToAnimate);
            animation.From = fromX;
            animation.To = toX;
            SineEase easeMode = new SineEase();
            easeMode.EasingMode = EasingMode.EaseIn;
            animation.EasingFunction = easeMode;
            
            animation.Duration = TimeSpan.FromMilliseconds(mSecInterval);
            return animation;
        }


        #endregion
        
    }

}
