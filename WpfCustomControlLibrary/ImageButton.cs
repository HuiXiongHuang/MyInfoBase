using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFCustomControlLibrary
{
    public class ImageButton : Button
    {

        //图标     
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(ImageButton), null);
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }


    }
}
