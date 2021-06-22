using Microsoft.Toolkit.Uwp.Notifications;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using StampImages.Core;
using System;
using System.Drawing;
using System.IO;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace StampImages.App.WPF.ViewModels
{
    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        StampImageFactory _stampImageFactory = new StampImageFactory(new Core.StampImageFactoryConfig());

        public ReactiveProperty<string> Title { get; } = new ReactiveProperty<string>("StampImages");

        public ReactiveProperty<string> TopText { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> MiddleText { get; } = new ReactiveProperty<string>(DateTime.Now.ToString("yyyy.MM.dd"));
        public ReactiveProperty<string> BottomText { get; } = new ReactiveProperty<string>();


        public ReactiveProperty<Bitmap> StampImage { get; } = new ReactiveProperty<Bitmap>();
        public ReactiveProperty<BitmapSource> StampImageSource { get; } = new ReactiveProperty<BitmapSource>();



        public DelegateCommand LoadedCommand { get; }

        public DelegateCommand CopyImageCommand { get; }

        /// <summary>
        /// コンストラクター
        /// </summary>
        public MainWindowViewModel()
        {
            LoadedCommand = new DelegateCommand(ExecuteLoadedCommand);
            CopyImageCommand = new DelegateCommand(ExecuteCopyImageCommand);


            TopText.Delay(TimeSpan.FromMilliseconds(500)).Subscribe(_ =>
            {
                UpdateStampImage();
            });
            MiddleText.Delay(TimeSpan.FromMilliseconds(500)).Subscribe(_ =>
            {
                UpdateStampImage();
            });
            BottomText.Delay(TimeSpan.FromMilliseconds(500)).Subscribe(_ =>
            {
                UpdateStampImage();
            });
        }

        /// <summary>
        /// 画面ロードコマンド
        /// </summary>
        private void ExecuteLoadedCommand()
        {
                
        }

        /// <summary>
        /// プレビュー画像コピーコマンド
        /// </summary>
        private void ExecuteCopyImageCommand()
        {
            if (StampImageSource == null)
            {
                return;
            }

            var resized = _stampImageFactory.Resize(StampImage.Value, 128, 128);
            var source = ConvertToBitmapSource(resized);
            PngBitmapEncoder pngEnc = new PngBitmapEncoder();
            pngEnc.Frames.Add(BitmapFrame.Create(source));

            using var ms = new MemoryStream();
            pngEnc.Save(ms);
            Clipboard.SetData("PNG", ms);

            new ToastContentBuilder()
                .AddText("クリップボードにコピーしました")
                .Show();

        }

        /// <summary>
        /// プレビュー画像を更新します。
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void UpdateStampImage()
        {
            var stamp = new Stamp
            {
                TopText = new StampText { Value = TopText.Value, Font = StampText.GetDefaultFont(22) },
                MiddleText = new StampText { Value = MiddleText.Value, Font = StampText.GetDefaultFont(30) },
                BottomText = new StampText { Value = BottomText.Value, Font = StampText.GetDefaultFont(25) }
            };
            var stampImage = _stampImageFactory.Create(stamp);

            Application.Current.Dispatcher.Invoke(() =>
            {
                StampImage.Value = stampImage;
                StampImageSource.Value = ConvertToBitmapSource(StampImage.Value);
            });
        }


        private BitmapSource ConvertToBitmapSource(Bitmap bitmap)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
