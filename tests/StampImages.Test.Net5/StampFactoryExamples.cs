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

        Core.StampImageFactory stampImageFactory = new Core.StampImageFactory(new Core.StampImageFactoryConfig());

        ~StampFactoryExamples()
        {
            stampImageFactory.Dispose();
        }

        /// <summary>
        /// 日付スタンプ出力
        /// </summary>
        [TestMethod]
        public void ExampleCreateThreeAreaCircularStamp()
        {
            var stamp = new ThreeAreaCircularStamp
            {
                TopText = new StampText { Value = "所属部門", Size = 22 },
                MiddleText = new StampText { Value = DateTime.Now.ToString("yyyy.MM.dd"), Size = 30 },
                BottomText = new StampText { Value = "ユーザー名", Size = 25 }
            };

            stampImageFactory.Save(stamp, "./inkan_256.png");
        }

        /// <summary>
        /// 四角形スタンプ出力
        /// </summary>
        [TestMethod]
        public void ExampleCreateSquareStamp()
        {
            var stamp = new RectangleStamp
            {
                EdgeType = StampEdgeType.Double,
                TextOrientationType = TextOrientationType.Vertical,
                Text = new StampText { Value = "承認", Size = 60 },
            };
            stamp.EffectTypes.Add(StampEffectType.Noise);

            using (var bitmap = stampImageFactory.Create(stamp))
            {
                bitmap.Save("./inkan_sq_256.png");
            }
        }

        [TestMethod]
        public void ExampleCreateRectangleStamp()
        {
            var stamp = new RectangleStamp
            {
                Size = new Size(512, 110),
                Color = Color.DarkCyan,
                IsFillColor = true,
                EdgeType = StampEdgeType.Double,
                EdgeWidth = 5,
                EdgeRadius = 0,

                Text = new StampText { Value = "SOLD OUT", Size = 70 },
            };
            stamp.EffectTypes.Add(StampEffectType.Grunge);

            using (var bitmap = stampImageFactory.Create(stamp))
            {
                bitmap.Save("./stamp_sold_out.png");
            }
        }

        /// <summary>
        /// 円形スタンプ出力
        /// </summary>
        [TestMethod]
        public void ExampleCreateCircularStamp()
        {
            var stamp = new CircularStamp
            {
                Text = new StampText { Value = "承認", Size = 60 },
            };

            using (var bitmap = stampImageFactory.Create(stamp))
            {
                bitmap.Save("./inkan_circular_256.png");
            }
        }



        /// <summary>
        /// リサイズ
        /// </summary>
        [TestMethod]
        public void ExampleResizeStamp()
        {
            var stamp = new ThreeAreaCircularStamp
            {
                TopText = new StampText { Value = "所属部門", Size = 22 },
                MiddleText = new StampText { Value = DateTime.Now.ToString("yyyy.MM.dd"), Size = 30 },
                BottomText = new StampText { Value = "ユーザー名", Size = 25 }
            };

            using (Bitmap stampImage = stampImageFactory.Create(stamp))
            using (Bitmap resized = stampImageFactory.Resize(stampImage, 128, 128))
            {
                resized.Save("./inkan_128.png", ImageFormat.Png);
            }

        }
    }
}
