using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
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
using System.Windows.Threading;

namespace StampImages.App.WPF.ViewModels
{
    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        private readonly StampImageFactory stampImageFactory = new StampImageFactory(new Core.StampImageFactoryConfig());

        private bool isInitialized = false;

        private readonly DispatcherTimer timer;


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
        public ReactiveProperty<System.Windows.Media.Color> StampColor { get; }
            = new ReactiveProperty<System.Windows.Media.Color>(System.Windows.Media.Color.FromRgb(Stamp.DEFAULT_STAMP_COLOR.R, Stamp.DEFAULT_STAMP_COLOR.G, Stamp.DEFAULT_STAMP_COLOR.B));

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
        /// 初期化コマンド
        /// </summary>
        public DelegateCommand ClearCommand { get; }
        /// <summary>
        /// 回転クリア
        /// </summary>
        public DelegateCommand ClearRotationCommand { get; }


        public DelegateCommand SaveImageCommand { get; }

        /// <summary>
        /// コンストラクター
        /// </summary>
        public MainWindowViewModel()
        {
            this.timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                UpdateStampImage();
            };

            LoadedCommand = new DelegateCommand(ExecuteLoadedCommand);
            CopyImageCommand = new DelegateCommand(ExecuteCopyImageCommand);
            ClearCommand = new DelegateCommand(ExecuteClearCommand);
            ClearRotationCommand = new DelegateCommand(ExecuteClearRotationCommand);
            SaveImageCommand = new DelegateCommand(ExecuteSaveImageCommand);

            TopText.Subscribe(_ =>
            {
                RequestUpdateStampImage();
            });
            MiddleText.Subscribe(_ =>
            {
                RequestUpdateStampImage();
            });
            BottomText.Subscribe(_ =>
            {
                RequestUpdateStampImage();
            });
            IsDoubleStampEdge.Subscribe(_ =>
            {
                RequestUpdateStampImage();
            });
            RotationAngle.Subscribe(_ =>
            {
                RequestUpdateStampImage();
            });
            IsAppendNoise.Subscribe(_ =>
            {
                RequestUpdateStampImage();
            });
            StampColor.Subscribe(_ =>
            {
                RequestUpdateStampImage();
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


        private void ExecuteClearCommand()
        {
            this.isInitialized = false;

            TopText.Value = null;
            MiddleText.Value = DateTime.Now.ToString("yyyy.MM.dd");
            BottomText.Value = null;

            RotationAngle.Value = 0;
            IsAppendNoise.Value = false;
            IsDoubleStampEdge.Value = false;

            StampColor.Value = System.Windows.Media.Color.FromRgb(Stamp.DEFAULT_STAMP_COLOR.R, Stamp.DEFAULT_STAMP_COLOR.G, Stamp.DEFAULT_STAMP_COLOR.B);

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

        private void RequestUpdateStampImage()
        {
            if (!timer.IsEnabled)
            {
                timer.Start();
            }
        }

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
            var stamp = new Stamp
            {
                TopText = new StampText { Value = TopText.Value, Font = StampText.GetDefaultFont(22) },
                MiddleText = new StampText { Value = MiddleText.Value, Font = StampText.GetDefaultFont(30) },
                BottomText = new StampText { Value = BottomText.Value, Font = StampText.GetDefaultFont(25) }
            };

            var option = stamp.Option;
            option.IsDoubleStampEdge = IsDoubleStampEdge.Value;
            option.RotationAngle = RotationAngle.Value;
            option.IsAppendNoise = IsAppendNoise.Value;

            Color drawingColor = Color.FromArgb(StampColor.Value.A, StampColor.Value.R, StampColor.Value.G, StampColor.Value.B);
            option.Pen.Color = drawingColor;
            stamp.SetBrush(StampText.GetDefaultBrush(drawingColor));


            var stampImage = this.stampImageFactory.Create(stamp);

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
