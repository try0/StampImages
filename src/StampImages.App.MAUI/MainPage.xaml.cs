
using Microsoft.Extensions.Configuration;
using SkiaSharp;
using StampImages.Core;
using StampImages.Core.SkiaSharp;
using System.Reflection;
using Font = Microsoft.Maui.Graphics.Font;

namespace StampImages.App.MAUI
{
    public partial class MainPage : ContentPage
    {
        private StampImageFactory stampImageFactory = new StampImageFactory(new StampImageFactoryConfig()
        {
            SKTypefaceProvider = fn =>
            {
                var res = Assembly.GetExecutingAssembly().GetManifestResourceStream("StampImages.App.MAUI.Resources.Fonts.NotoSansJP-Regular.otf");
                return SKTypeface.FromStream(res);
            }
        });

        private bool init = true;
        private Random random = new Random();
        private Stream stream;

        public MainPage()
        {
            InitializeComponent();

            RenderStampImageAsync();

        }

        private void OnCounterClicked(object sender, EventArgs e)
        {

            UpdateBtn.IsEnabled = false;
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

            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }

            stream = await Task.Run(() =>
            {
                SKBitmap bitmap = stampImageFactory.Create(stamp);
                SKImage image = SKImage.FromBitmap(bitmap);
                return image.Encode().AsStream();
            });

            img.Source = ImageSource.FromStream(() => stream);

            UpdateBtn.IsEnabled = true;
            init = false;
        }
    }
}