using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using StampImages.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interop;
using Media = System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Media;

namespace StampImages.App.WPF.ViewModels
{
    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public abstract class StampPanelBaseViewModel : BindableBase
    {


        private readonly StampImageFactory stampImageFactory = new StampImageFactory(new Core.StampImageFactoryConfig());

        protected bool isInitialized = false;

        private readonly DispatcherTimer timer;


        /// <summary>
        /// 2重円
        /// </summary>
        public ReactiveProperty<bool> IsDoubleStampEdge { get; } = new ReactiveProperty<bool>(false);

        /// <summary>
        /// 回転角度
        /// </summary>
        public ReactiveProperty<int> RotationAngle { get; } = new ReactiveProperty<int>(0);

        /// <summary>
        /// ノイズ付与
        /// </summary>
        public ReactiveProperty<bool> IsAppendNoise { get; } = new ReactiveProperty<bool>(false);
        /// <summary>
        /// スタンプカラー
        /// </summary>
        public ReactiveProperty<Media.Color> StampColor { get; }
            = new ReactiveProperty<Media.Color>(Media.Color.FromRgb(ThreeAreaCircularStamp.DEFAULT_STAMP_COLOR.R, ThreeAreaCircularStamp.DEFAULT_STAMP_COLOR.G, ThreeAreaCircularStamp.DEFAULT_STAMP_COLOR.B));

        /// <summary>
        /// フォント一覧
        /// </summary>
        public ReactiveProperty<ICollection<Media.FontFamily>> SystemFontFamilies { get; }
            = new ReactiveProperty<ICollection<Media.FontFamily>>(Fonts.SystemFontFamilies);
        /// <summary>
        /// 描画フォント
        /// </summary>
        public ReactiveProperty<Media.FontFamily> FontFamily { get; set; }
            = new ReactiveProperty<Media.FontFamily>(new Media.FontFamily("MS UI Gothic"));
        /// <summary>
        /// プレビュー画像
        /// </summary>
        public ReactiveProperty<Bitmap> StampImage { get; } = new ReactiveProperty<Bitmap>();
        /// <summary>
        /// プレビュー画像イメージソース
        /// </summary>
        public ReactiveProperty<BitmapSource> StampImageSource { get; } = new ReactiveProperty<BitmapSource>();



        /// <summary>
        /// 画像コピーコマンド
        /// </summary>
        public DelegateCommand CopyImageCommand { get; }
        /// <summary>
        /// 初期化コマンド
        /// </summary>
        public DelegateCommand ClearCommand { get; }
        /// <summary>
        /// 回転クリア
        /// </summary>
        public DelegateCommand ClearRotationCommand { get; }
        /// <summary>
        /// 画像保存
        /// </summary>
        public DelegateCommand SaveImageCommand { get; }

        /// <summary>
        /// コンストラクター
        /// </summary>
        public StampPanelBaseViewModel()
        {
            this.timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                UpdateStampImage();
            };

            CopyImageCommand = new DelegateCommand(ExecuteCopyImageCommand);
            ClearCommand = new DelegateCommand(ExecuteClearCommand);
            ClearRotationCommand = new DelegateCommand(ExecuteClearRotationCommand);
            SaveImageCommand = new DelegateCommand(ExecuteSaveImageCommand);


            IsDoubleStampEdge.Subscribe(_ => RequestUpdateStampImage());
            RotationAngle.Subscribe(_ => RequestUpdateStampImage());
            IsAppendNoise.Subscribe(_ => RequestUpdateStampImage());
            StampColor.Subscribe(_ => RequestUpdateStampImage());
            FontFamily.Subscribe(_ => RequestUpdateStampImage());

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

            this.isInitialized = true;
            UpdateStampImage();
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

            var resized = this.stampImageFactory.Resize(StampImage.Value, 128, 128);
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


        protected virtual void ExecuteClearCommand()
        {
            this.isInitialized = false;



            RotationAngle.Value = 0;
            IsAppendNoise.Value = false;
            IsDoubleStampEdge.Value = false;

            FontFamily.Value = new Media.FontFamily("MS UI Gothic");

            StampColor.Value = Media.Color.FromRgb(ThreeAreaCircularStamp.DEFAULT_STAMP_COLOR.R, ThreeAreaCircularStamp.DEFAULT_STAMP_COLOR.G, ThreeAreaCircularStamp.DEFAULT_STAMP_COLOR.B);

            this.isInitialized = true;
            UpdateStampImage();

        }

        private void ExecuteClearRotationCommand()
        {
            RotationAngle.Value = 0;
        }

        private void ExecuteSaveImageCommand()
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = "stamp.png";
            dialog.Filter = "PNGファイル(*.png)|*.png";

            // ファイル保存ダイアログを表示します。
            var result = dialog.ShowDialog() ?? false;

            // 保存ボタン以外が押下された場合
            if (!result)
            {
                // 終了します。
                return;
            }

            var isDirectory = File
                .GetAttributes(dialog.FileName)
                .HasFlag(FileAttributes.Directory);

            if (isDirectory)
            {
                dialog.FileName = Path.Combine(dialog.FileName, "stamp.png");
            }

            StampImage.Value.Save(dialog.FileName);

            new ToastContentBuilder()
                .AddAudio(new ToastAudio() { Silent = true })
                .AddInlineImage(new Uri(dialog.FileName))
                .AddText("保存しました")
                .Show();
        }

        public void RequestUpdateStampImage()
        {
            if (!timer.IsEnabled)
            {
                timer.Start();
            }
        }

        protected abstract BaseStamp NewStamp();

        /// <summary>
        /// プレビュー画像を更新します。
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void UpdateStampImage()
        {
            if (!this.isInitialized)
            {
                return;
            }
            var stamp = NewStamp();

            stamp.SetFontFamily(new System.Drawing.FontFamily(FontFamily.Value.Source));


            stamp.EdgeType = IsDoubleStampEdge.Value ? StampEdgeType.DOUBLE : StampEdgeType.SINGLE;
            stamp.RotationAngle = RotationAngle.Value;
            if (IsAppendNoise.Value)
            {
                stamp.EffectTypes.Add(StampEffectType.NOISE);
            }

            System.Drawing.Color drawingColor = System.Drawing.Color.FromArgb(StampColor.Value.A, StampColor.Value.R, StampColor.Value.G, StampColor.Value.B);
            stamp.Color = drawingColor;


            Bitmap stampImage = null;
            if (stamp is ThreeAreaCircularStamp)
            {
                stampImage = this.stampImageFactory.Create((ThreeAreaCircularStamp)stamp);
            }
            else if (stamp is SquareStamp)
            {
                stampImage = this.stampImageFactory.Create((SquareStamp)stamp);
            }

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
