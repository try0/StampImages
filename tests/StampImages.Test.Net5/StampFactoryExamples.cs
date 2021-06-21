using Microsoft.VisualStudio.TestTools.UnitTesting;
using StampImages.Core;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace StampImages.Test.Net5
{
    [TestClass]
    public class StampFactoryExamples
    {

        StampImageFactory stampImageFactory = new StampImageFactory(new Core.StampImageFactoryConfig());

        [TestMethod]
        public void ExampleCreateStamp()
        {
            var stamp = new Stamp
            {
                TopText = new StampText { Value = "所属部門", Font = StampText.GetDefaultFont(22) },
                MiddleText = new StampText { Value = DateTime.Now.ToString("yyyy.MM.dd"), Font = StampText.GetDefaultFont(30) },
                BottomText = new StampText { Value = "ユーザー名", Font = StampText.GetDefaultFont(25) }
            };
            stampImageFactory.Save(stamp, "./inkan_256.png");
        }

        [TestMethod]
        public void ExampleResizeStamp()
        {
            var stamp = new Stamp
            {
                TopText = new StampText { Value = "所属部門", Font = StampText.GetDefaultFont(22) },
                MiddleText = new StampText { Value = DateTime.Now.ToString("yyyy.MM.dd"), Font = StampText.GetDefaultFont(30) },
                BottomText = new StampText { Value = "ユーザー名", Font = StampText.GetDefaultFont(25) }
            };

            
            Bitmap stampImage = stampImageFactory.Create(stamp);
            Bitmap resized = stampImageFactory.Resize(stampImage, 128, 128);

            resized.Save("./inkan_128.png", ImageFormat.Png);

            stampImage.Dispose();
            resized.Dispose();

        }
    }
}
