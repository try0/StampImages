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
using StampImages.App.WPF.Services;
using System.Drawing.Imaging;
using System.Windows.Input;
using System.Diagnostics;
using System.Linq;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using ControlzEx.Theming;

namespace StampImages.App.WPF.ViewModels
{
    /// <summary>
    /// StampPanelBaseViewModel
    /// </summary>
    public abstract class StampPanelBaseViewModel : BindableBase
    {

        private const int MAKER_NOTE_ID = 0x927C;

        private readonly StampImageFactory stampImageFactory = new StampImageFactory(new StampImageFactoryConfig());

        protected bool isInitialized = false;

        private readonly DispatcherTimer timer;


        public IConfigurationService ConfigurationService { get; set; }

        protected abstract Type StampType { get; }

        /// <summary>
        /// 2重円
        /// </summary>
        public ReactiveProperty<bool> IsDoubleStampEdge { get; } = new ReactiveProperty<bool>(false);

        /// <summary>
        /// 回転角度
        /// </summary>
        public ReactiveProperty<int> RotationAngle { get; } = new ReactiveProperty<int>(0);

        /// <summary>
        /// 塗りつぶし
        /// </summary>
        public ReactiveProperty<bool> IsFillColor { get; } = new ReactiveProperty<bool>(false);

        /// <summary>
        /// ノイズ付与
        /// </summary>
        public ReactiveProperty<bool> IsAppendNoise { get; } = new ReactiveProperty<bool>(false);
        /// <summary>
        /// 汚し加工付与
        /// </summary>
        public ReactiveProperty<bool> IsAppendGrunge { get; } = new ReactiveProperty<bool>(false);
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
        /// 表示中スタンプデータ
        /// </summary>
        public ReactiveProperty<BaseStamp> Stamp { get; } = new ReactiveProperty<BaseStamp>();
        /// <summary>
        /// プレビュー画像
        /// </summary>
        public ReactiveProperty<Bitmap> StampImage { get; } = new ReactiveProperty<Bitmap>();
        /// <summary>
        /// プレビュー画像イメージソース
        /// </summary>
        public ReactiveProperty<BitmapSource> StampImageSource { get; } = new ReactiveProperty<BitmapSource>();


        public DelegateCommand LoadedCommand { get; }

        public DelegateCommand UnloadedCommand { get; }

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
        /// 画像ドラッグ
        /// </summary>
        public DelegateCommand<MouseEventArgs> DragImageCommand { get; }
        /// <summary>
        /// 画像ドロップ
        /// </summary>
        public DelegateCommand<DragEventArgs> DropItemCommand { get; }


        public IDialogCoordinator MahAppsDialogCoordinator { get; set; }


        /// <summary>
        /// コンストラクター
        /// </summary>
        public StampPanelBaseViewModel(IConfigurationService cs)
        {
            this.ConfigurationService = cs;
            this.timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                UpdateStampImage();
            };

            LoadedCommand = new DelegateCommand(ExecuteLoadedCommand);
            UnloadedCommand = new DelegateCommand(ExecuteUnloadedCommnad);
            CopyImageCommand = new DelegateCommand(ExecuteCopyImageCommand);
            ClearCommand = new DelegateCommand(ExecuteClearConfirmCommand);
            ClearRotationCommand = new DelegateCommand(ExecuteClearRotationCommand);
            SaveImageCommand = new DelegateCommand(ExecuteSaveImageCommand);
            DragImageCommand = new DelegateCommand<MouseEventArgs>(ExecuteDragImageCommand);
            DropItemCommand = new DelegateCommand<DragEventArgs>(ExecuteDropItemCommand);

            BaseStamp stamp = ConfigurationService.Load(StampType);
            if (stamp != null)
            {
                LoadStamp(stamp);
                stamp.Dispose();
            }
            IsFillColor.Subscribe(_ => RequestUpdateStampImage());
            IsDoubleStampEdge.Subscribe(_ => RequestUpdateStampImage());
            RotationAngle.Subscribe(_ => RequestUpdateStampImage());
            IsAppendNoise.Subscribe(_ => RequestUpdateStampImage());
            IsAppendGrunge.Subscribe(_ => RequestUpdateStampImage());
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
        /// スタンプデータを生成します。
        /// </summary>
        /// <returns></returns>
        protected abstract BaseStamp NewStamp();

        /// <summary>
        /// 保存されているスタンププロパティーを復元します。
        /// </summary>
        /// <param name="stamp"></param>
        protected virtual void LoadStamp(BaseStamp stamp)
        {
            IsFillColor.Value = stamp.IsFillColor;
            IsDoubleStampEdge.Value = stamp.EdgeType == StampEdgeType.Double;
            RotationAngle.Value = stamp.RotationAngle;
            IsAppendNoise.Value = stamp.EffectTypes.Contains(StampEffectType.Noise);
            IsAppendGrunge.Value = stamp.EffectTypes.Contains(StampEffectType.Grunge);
            StampColor.Value = Media.Color.FromRgb(stamp.Color.R, stamp.Color.G, stamp.Color.B);
        }

        /// <summary>
        /// 画面ロードコマンド
        /// </summary>
        private void ExecuteLoadedCommand()
        {
        }

        /// <summary>
        /// 画面アンロードコマンド
        /// </summary>
        private void ExecuteUnloadedCommnad()
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

            using (var resized = this.stampImageFactory.Resize(StampImage.Value, 128, 128))
            {
                PngBitmapEncoder pngEnc = GetEncoder(resized);
                using (var ms = new MemoryStream())
                {
                    pngEnc.Save(ms);
                    Clipboard.SetData("PNG", ms);
                }
            }

            new ToastContentBuilder()
                .AddAudio(new ToastAudio() { Silent = true })
                .AddText("クリップボードにコピーしました")
                .Show();
        }

        protected virtual async void ExecuteClearConfirmCommand()
        {
            string messageBoxText = "設定内容を破棄します。よろしいですか?";
            string caption = "確認";

            // TODO Interface
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            var mySettings = new MetroDialogSettings()
            {
                ColorScheme = MetroDialogColorScheme.Accented
            };
            var select = await metroWindow.ShowMessageAsync(caption, messageBoxText, MessageDialogStyle.AffirmativeAndNegative, mySettings);
            if (select == MessageDialogResult.Affirmative)
            {
                ExecuteClearCommand();
            }

        }

        /// <summary>
        /// 設定値初期化コマンド
        /// </summary>
        protected virtual void ExecuteClearCommand()
        {
            this.isInitialized = false;

            RotationAngle.Value = 0;
            IsAppendNoise.Value = false;
            IsAppendGrunge.Value = false;
            IsFillColor.Value = false;
            IsDoubleStampEdge.Value = false;

            FontFamily.Value = new Media.FontFamily("MS UI Gothic");

            StampColor.Value = Media.Color.FromRgb(ThreeAreaCircularStamp.DEFAULT_STAMP_COLOR.R, ThreeAreaCircularStamp.DEFAULT_STAMP_COLOR.G, ThreeAreaCircularStamp.DEFAULT_STAMP_COLOR.B);

            this.isInitialized = true;
            UpdateStampImage();

        }

        /// <summary>
        /// 回転クリアコマンド
        /// </summary>
        private void ExecuteClearRotationCommand()
        {
            RotationAngle.Value = 0;
        }

        /// <summary>
        /// ファイル保存コマンド
        /// </summary>
        private void ExecuteSaveImageCommand()
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = "stamp.png";
            dialog.Filter = "PNGファイル(*.png)|*.png";

            var result = dialog.ShowDialog() ?? false;


            if (!result)
            {
                return;
            }

            if (File.Exists(dialog.FileName))
            {
                var isDirectory = File
                    .GetAttributes(dialog.FileName)
                    .HasFlag(FileAttributes.Directory);

                if (isDirectory)
                {
                    dialog.FileName = Path.Combine(dialog.FileName, "stamp.png");
                }
            }

            string fileName = dialog.FileName;
            if (!fileName.ToLower().EndsWith(".png"))
            {
                fileName += ".png";
            }

            StampImage.Value.Save(fileName);

            new ToastContentBuilder()
                .AddAudio(new ToastAudio() { Silent = true })
                .AddInlineImage(new Uri(fileName))
                .AddText("保存しました")
                .Show();
        }

        /// <summary>
        /// 画像ドラッグコマンド
        /// </summary>
        /// <param name="e"></param>
        private void ExecuteDragImageCommand(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                using (var resized = stampImageFactory.Resize(StampImage.Value, 128, 128))
                {

                    var tempPath = Path.GetTempPath();
                    Directory.CreateDirectory(tempPath);

                    var imagePath = Path.Combine(tempPath, $"stamp-{DateTime.Now.ToString("yyyyMMddHHmmss")}.png");

                    SaveStampImage(resized, imagePath);

                    string[] files = new string[1];
                    files[0] = imagePath;
                    DataObject data = new DataObject(DataFormats.FileDrop, files);

                    DragDrop.DoDragDrop(new UIElement(), data, DragDropEffects.Move);
                }
            }
        }

        /// <summary>
        /// ファイルドロップコマンド
        /// </summary>
        /// <param name="e"></param>
        private void ExecuteDropItemCommand(DragEventArgs e)
        {

            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null && files.Length > 0)
            {
                string file = files[0];

                if (!file.ToLower().EndsWith("png"))
                {
                    return;
                }


                var image = new Bitmap(file);

                string stampPropJson;
                if (image.PropertyIdList.Contains(MAKER_NOTE_ID))
                {
                    // pngにexifないけど、拡張したとき用
                    var propItem = image.GetPropertyItem(MAKER_NOTE_ID);
                    stampPropJson = System.Text.Encoding.UTF8.GetString(propItem.Value);
                }
                else
                {
                    // チャンク読み込み

                    Uri uri = new Uri(file, UriKind.Absolute);
                    BitmapFrame frame = BitmapFrame.Create(uri);
                    BitmapMetadata metaData = (BitmapMetadata)frame.Metadata.Clone();

                    stampPropJson = metaData.GetQuery("/tEXt/{str=Comment}").ToString();
                }

                if (String.IsNullOrEmpty(stampPropJson))
                {
                    return;
                }


                BaseStamp stamp = ConfigurationService.Deserialize<BaseStamp>(stampPropJson, StampType);
                if (stamp != null)
                {
                    LoadStamp(stamp);
                    stamp.Dispose();
                }

            }


        }

        public void RequestUpdateStampImage()
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
            var stamp = NewStamp();

            stamp.SetFontFamily(new System.Drawing.FontFamily(FontFamily.Value.Source));

            stamp.IsFillColor = IsFillColor.Value;
            stamp.EdgeType = IsDoubleStampEdge.Value ? StampEdgeType.Double : StampEdgeType.Single;
            stamp.RotationAngle = RotationAngle.Value;
            if (IsAppendNoise.Value)
            {
                stamp.EffectTypes.Add(StampEffectType.Noise);
            }

            if (IsAppendGrunge.Value)
            {
                stamp.EffectTypes.Add(StampEffectType.Grunge);
            }

            System.Drawing.Color drawingColor = System.Drawing.Color.FromArgb(StampColor.Value.A, StampColor.Value.R, StampColor.Value.G, StampColor.Value.B);
            stamp.Color = drawingColor;


            Bitmap stampImage = this.stampImageFactory.Create(stamp);
            ConfigurationService.Save(stamp);

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Stamp.Value != null)
                {
                    Stamp.Value.Dispose();
                }

                if (StampImage.Value != null)
                {
                    StampImage.Value.Dispose();
                }

                Stamp.Value = stamp;
                StampImage.Value = stampImage;
            });
        }

        private void FlashStampProperties(Bitmap image, BaseStamp stamp)
        {
            //　TODO PNGはexif持てない　保存形式拡張したときに使う
            try
            {
                if (!File.Exists("./tmp.png"))
                {
                    // PropertyItem取得用にキープ
                    image.Save("./tmp.png", ImageFormat.Png);
                }


                var stampPropJson = ConfigurationService.Serialize(stamp);
                PropertyItem propItem = new Bitmap("./tmp.png").PropertyItems[0];
                propItem.Id = MAKER_NOTE_ID;
                propItem.Value = System.Text.Encoding.UTF8.GetBytes(stampPropJson);
                image.SetPropertyItem(propItem);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

        }

        private PngBitmapEncoder GetEncoder(Bitmap bitmap)
        {
            var source = ConvertToBitmapSource(bitmap);
            PngBitmapEncoder pngEnc = new PngBitmapEncoder();
            var metadata = new BitmapMetadata("png");
            metadata.SetQuery("/tEXt/{str=Comment}", ConfigurationService.Serialize(Stamp.Value));
            var frame = BitmapFrame.Create(source, null, metadata, null);
            pngEnc.Frames.Add(frame);

            return pngEnc;
        }

        private void SaveStampImage(Bitmap bitmap, string imagePath)
        {
            PngBitmapEncoder pngEnc = GetEncoder(bitmap);
            using (FileStream fs = new FileStream(imagePath, FileMode.Create))
            {
                pngEnc.Save(fs);
            }
        }

        private BitmapSource ConvertToBitmapSource(Bitmap bitmap)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

    }
}
