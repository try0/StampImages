
using Microsoft.Extensions.Configuration;
using SkiaSharp;
using StampImages.App.MAUI.Util;
using StampImages.Core;
using StampImages.Core.SkiaSharp;
using System.Reflection;
using Font = Microsoft.Maui.Graphics.Font;

namespace StampImages.App.MAUI
{
    public partial class MainPage : ContentPage
    {

        private StampImageFactory stampImageFactory;
        private FontProvider fontProvider = new FontProvider();
        private bool init = true;
        private Random random = new Random();
        private Stream stream;


        public MainPage()
        {
            stampImageFactory = new StampImageFactory(new StampImageFactoryConfig()
            {
                SKTypefaceProvider = fn =>
                {

                    if (FontPicker.SelectedItem != null)
                    {
                        var name = FontPicker.SelectedItem.ToString();

#if ANDROID
                        var path = fontProvider.GetFilePath(name);
                        return SKTypeface.FromFile(path);
#else
                        return SKTypeface.FromFamilyName(name);
#endif

                    }

                    // ドキュメントには、MauiAsset、 Resources/Rawについてしか記載がないが、
                    // ビルドアクションがMauiFontでもOpenAppPackageFileAsyncで読めるっぽい
                    // https://docs.microsoft.com/ja-jp/dotnet/maui/platform-integration/storage/file-system-helpers?tabs=windows#platform-differences
                    var stream = FileSystem.Current.OpenAppPackageFileAsync("NotoSansJP-Regular.otf").Result;

                    // 埋め込みリソースなら↓
                    //var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("StampImages.App.MAUI.Resources.Fonts.NotoSansJP-Regular.otf");

                    return SKTypeface.FromStream(stream);
                }
            });

            InitializeComponent();

            FontPicker.ItemsSource = fontProvider.GetInstalledFontFamilyNames();

            RenderStampImageAsync();

        }


        private void OnUpdateClicked(object sender, EventArgs e)
        {

            UpdateBtn.IsEnabled = false;
            RenderStampImageAsync();
        }

        private void OnFontChanged(object sender, EventArgs e)
        {
            FontPicker.IsEnabled = false;
            RenderStampImageAsync();
        }

        /// <summary>
        /// 画像を生成・描画します。
        /// </summary>
        private async void RenderStampImageAsync()
        {


            var stamp = new RectangleStamp
            {
                Size = new System.Drawing.Size(500, 150),
                Color = !init ? System.Drawing.Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)) : System.Drawing.ColorTranslator.FromHtml("#512BD4"),
                IsFillColor = random.Next(2) == 1,
                EdgeType = random.Next(2) == 1 ? StampEdgeType.Single : StampEdgeType.Double,
                EdgeWidth = 5,
                EdgeRadius = 0,

                Text = new StampText { Value = "StampImages", Size = 70 },
            };

            if (random.Next(2) == 1)
            {
                stamp.EffectTypes.Add(StampEffectType.Grunge);
            }
            if (random.Next(2) == 1)
            {
                stamp.EffectTypes.Add(StampEffectType.Noise);
            }

            try
            {
                if (stream != null)
                {
                    stream.Dispose();
                    stream = null;
                }
            }
            catch (Exception ignore)
            {
            }


            stream = await Task.Run(() =>
            {
                SKBitmap bitmap = stampImageFactory.Create(stamp);
                SKImage image = SKImage.FromBitmap(bitmap);
                return image.Encode().AsStream();
            });

            img.Source = ImageSource.FromStream(() => stream);

            UpdateBtn.IsEnabled = true;
            FontPicker.IsEnabled = true;
            init = false;
        }


    }
}