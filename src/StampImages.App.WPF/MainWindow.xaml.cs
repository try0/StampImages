using Microsoft.Toolkit.Uwp.Notifications;
using StampImages.Core;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace StampImages.App.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StampImageFactory stampImageFactory = new StampImageFactory(new Core.StampImageFactoryConfig());

        private Bitmap DisplayStampBitmap { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Closing += MainWindow_Closing;
        }


        private void btnGenerateStamp_Click(object sender, RoutedEventArgs e)
        {
            var stamp = new Stamp
            {
                TopText = new StampText { Value = "所属部門", Font = StampText.GetDefaultFont(22) },
                MiddleText = new StampText { Value = DateTime.Now.ToString("yyyy.MM.dd"), Font = StampText.GetDefaultFont(30) },
                BottomText = new StampText { Value = "ユーザー名", Font = StampText.GetDefaultFont(25) }
            };
            var stampImage = stampImageFactory.Create(stamp);
            DisplayStampBitmap = stampImage;
            this.imgStamp.Source = ConvertToBitmapSource(DisplayStampBitmap);

        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayStampBitmap == null)
            {
                return;
            }

            var source = ConvertToBitmapSource(DisplayStampBitmap);
            PngBitmapEncoder pngEnc = new PngBitmapEncoder();
            pngEnc.Frames.Add(BitmapFrame.Create(source));

            using var ms = new MemoryStream();
            pngEnc.Save(ms);
            Clipboard.SetData("PNG", ms);

            new ToastContentBuilder()
                .AddText("クリップボードにコピーしました")
                .Show();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DisplayStampBitmap != null)
            {
                DisplayStampBitmap.Dispose();
            }

            stampImageFactory.Dispose();
        }


        /// <summary>
        /// BitmapSourceへ変換します。
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        private BitmapSource ConvertToBitmapSource(Bitmap bitmap)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }



    }
}
