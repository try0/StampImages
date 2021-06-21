using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace StampImages.Test.NetFramework
{
    [TestClass]
    public class StampFactoryExamples
    {

        StampFactory stampFactory = new StampFactory(new StampFactoryConfig());

        [TestMethod]
        public void ExampleCreateStamp()
        {
            var texts = new StampTexts
            {
                TopText = new StampText { Value = "所属部門", Font = StampText.GetDefaultFont(22) },
                MiddleText = new StampText { Value = DateTime.Now.ToString("yyyy.MM.dd"), Font = StampText.GetDefaultFont(30) },
                BottomText = new StampText { Value = "ユーザー名", Font = StampText.GetDefaultFont(25) }
            };
            stampFactory.Save(texts, "./inkan_256.png");
        }

        [TestMethod]
        public void ExampleResizeStamp()
        {
            var texts = new StampTexts
            {
                TopText = new StampText { Value = "所属部門", Font = StampText.GetDefaultFont(22) },
                MiddleText = new StampText { Value = DateTime.Now.ToString("yyyy.MM.dd"), Font = StampText.GetDefaultFont(30) },
                BottomText = new StampText { Value = "ユーザー名", Font = StampText.GetDefaultFont(25) }
            };

            
            Bitmap stampImage = stampFactory.Create(texts);
            Bitmap resized = stampFactory.Resize(stampImage, 128, 128);

            resized.Save("./inkan_128.png", ImageFormat.Png);

            stampImage.Dispose();
            resized.Dispose();

        }
    }
}
