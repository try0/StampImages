﻿using Microsoft.Toolkit.Uwp.Notifications;
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

        /// <summary>
        /// Windowタイトル
        /// </summary>
        public ReactiveProperty<string> Title { get; } = new ReactiveProperty<string>("StampImages");


        /// <summary>
        /// 上段テキスト
        /// </summary>
        public ReactiveProperty<string> TopText { get; } = new ReactiveProperty<string>();
        /// <summary>
        /// 中段テキスト
        /// </summary>
        public ReactiveProperty<string> MiddleText { get; } = new ReactiveProperty<string>(DateTime.Now.ToString("yyyy.MM.dd"));
        /// <summary>
        /// 下段テキスト
        /// </summary>
        public ReactiveProperty<string> BottomText { get; } = new ReactiveProperty<string>();

        /// <summary>
        /// 2重円
        /// </summary>
        public ReactiveProperty<bool> IsDoubleStampEdge { get; } = new ReactiveProperty<bool>(false);


        /// <summary>
        /// プレビュー画像
        /// </summary>
        public ReactiveProperty<Bitmap> StampImage { get; } = new ReactiveProperty<Bitmap>();
        public ReactiveProperty<BitmapSource> StampImageSource { get; } = new ReactiveProperty<BitmapSource>();


        /// <summary>
        /// 画面ロードコマンド
        /// </summary>
        public DelegateCommand LoadedCommand { get; }
        /// <summary>
        /// 画像コピーコマンド
        /// </summary>
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
            IsDoubleStampEdge.Delay(TimeSpan.FromMilliseconds(500)).Subscribe(_ =>
            {
                UpdateStampImage();
            });


            StampImage.Subscribe(img =>
            {
                if (img != null)
                {
                    StampImageSource.Value = ConvertToBitmapSource(StampImage.Value);
                }
                else
                {
                    StampImageSource.Value = null;
                }
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
                .AddAudio(new ToastAudio() { Silent = true })
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

            stamp.Option.IsDoubleStampEdge = IsDoubleStampEdge.Value;

            var stampImage = _stampImageFactory.Create(stamp);

            Application.Current.Dispatcher.Invoke(() =>
            {
                StampImage.Value = stampImage;

            });
        }


        private BitmapSource ConvertToBitmapSource(Bitmap bitmap)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
    }
}