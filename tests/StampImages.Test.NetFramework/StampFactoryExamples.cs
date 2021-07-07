using Microsoft.VisualStudio.TestTools.UnitTesting;
using StampImages.Core;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace StampImages.Test.NetFramework
{
    [TestClass]
    public class StampFactoryExamples
    {

        StampImageFactory stampImageFactory = new StampImageFactory(new Core.StampImageFactoryConfig());

        ~StampFactoryExamples()
        {
            stampImageFactory.Dispose();
        }

        /// <summary>
        /// ���t�X�^���v�o��
        /// </summary>
        [TestMethod]
        public void ExampleCreateThreeAreaCircularStamp()
        {
            var stamp = new ThreeAreaCircularStamp
            {
                TopText = new StampText { Value = "��������", Size = 22 },
                MiddleText = new StampText { Value = DateTime.Now.ToString("yyyy.MM.dd"), Size = 30 },
                BottomText = new StampText { Value = "���[�U�[��", Size = 25 }
            };

            using (stamp)
            {
                stampImageFactory.Save(stamp, "./inkan_256.png");
            }
        }

        /// <summary>
        /// �l�p�`�X�^���v�o��
        /// </summary>
        [TestMethod]
        public void ExampleCreateSquareStamp()
        {
            var stamp = new RectangleStamp
            {
                EdgeType = StampEdgeType.Double,
                TextOrientationType = TextOrientationType.Vertical,
                Text = new StampText { Value = "���F", Size = 60 },
            };
            stamp.EffectTypes.Add(StampEffectType.Noise);

            using (stamp)
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
                Size = new Size(512, 180),
                EdgeType = StampEdgeType.Single,
                EdgeWidth = 10,
                EdgeRadius = 0,
                Text = new StampText { Value = "SOLD OUT", Size = 70, IsIgnoreFontDescent = false },
            };
            stamp.EffectTypes.Add(StampEffectType.Noise);

            using (stamp)
            using (var bitmap = stampImageFactory.Create(stamp))
            {
                bitmap.Save("./stamp_sold_out.png");
            }
        }

        /// <summary>
        /// �~�`�X�^���v�o��
        /// </summary>
        [TestMethod]
        public void ExampleCreateCircularStamp()
        {
            var stamp = new CircularStamp
            {
                Text = new StampText { Value = "���F", Size = 60 },
            };

            using (stamp)
            using (var bitmap = stampImageFactory.Create(stamp))
            {
                bitmap.Save("./inkan_circular_256.png");
            }
        }



        /// <summary>
        /// ���T�C�Y
        /// </summary>
        [TestMethod]
        public void ExampleResizeStamp()
        {
            var stamp = new ThreeAreaCircularStamp
            {
                TopText = new StampText { Value = "��������", Size = 22 },
                MiddleText = new StampText { Value = DateTime.Now.ToString("yyyy.MM.dd"), Size = 30 },
                BottomText = new StampText { Value = "���[�U�[��", Size = 25 }
            };

            using (stamp)
            using (Bitmap stampImage = stampImageFactory.Create(stamp))
            using (Bitmap resized = stampImageFactory.Resize(stampImage, 128, 128))
            {
                resized.Save("./inkan_128.png", ImageFormat.Png);
            }

        }
    }
}
