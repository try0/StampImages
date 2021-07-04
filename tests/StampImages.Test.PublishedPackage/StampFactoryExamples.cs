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
            var stamp = new SquareStamp
            {
                EdgeType = StampEdgeType.DOUBLE,
                TextOrientationType = TextOrientationType.VERTICAL,
                Text = new StampText { Value = "���F", Size = 60 },
            };
            stamp.EffectTypes.Add(StampEffectType.NOISE);

            using (stamp)
            using (var bitmap = stampImageFactory.Create(stamp))
            {
                bitmap.Save("./inkan_sq_256.png");
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