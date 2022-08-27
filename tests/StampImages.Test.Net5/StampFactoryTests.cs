using Microsoft.VisualStudio.TestTools.UnitTesting;
using StampImages.Core;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace StampImages.Test.Net5
{
    [TestClass]
    public class StampFactoryTests
    {

        StampImageFactory stampImageFactory = new StampImageFactory(new Core.StampImageFactoryConfig());


        private void AssertSize(Size size, BaseStamp stamp)
        {

            using (var bitmap = stampImageFactory.Create(stamp))
            {
                Assert.AreEqual(size.Width, bitmap.Width);
                Assert.AreEqual(size.Height, bitmap.Height);
            }
        }


        [TestMethod]
        public void TestDefaultSize_ThreeAreaCircularStamp()
        {
            var stamp = new ThreeAreaCircularStamp();

            AssertSize(new Size(256, 256), stamp);
        }

        [TestMethod]
        public void TestDefaultSize_CircularStamp()
        {
            var stamp = new CircularStamp();

            AssertSize(new Size(256, 256), stamp);
        }

        [TestMethod]
        public void TestDefaultSize_RectangleStamp()
        {
            var stamp = new RectangleStamp();

            AssertSize(new Size(256, 256), stamp);
        }

        [TestMethod]
        public void TestSize_ThreeAreaCircularStamp()
        {
            AssertSize(new Size(110, 110), new ThreeAreaCircularStamp
            {
                Size = new Size(100, 100),
                Margin = new StampMargin(5, 5)
            });

            AssertSize(new Size(120, 110), new ThreeAreaCircularStamp
            {
                Size = new Size(110, 100),
                Margin = new StampMargin(5, 5)
            });

            AssertSize(new Size(130, 110), new ThreeAreaCircularStamp
            {
                Size = new Size(110, 100),
                Margin = new StampMargin(5, 10)
            });

            AssertSize(new Size(110, 120), new ThreeAreaCircularStamp
            {
                Size = new Size(100, 110),
                Margin = new StampMargin(5, 5)
            });

            AssertSize(new Size(110, 130), new ThreeAreaCircularStamp
            {
                Size = new Size(100, 110),
                Margin = new StampMargin(10, 5)
            });
        }

        [TestMethod]
        public void TestSize_CircularStamp()
        {
            AssertSize(new Size(110, 110), new CircularStamp
            {
                Size = new Size(100, 100),
                Margin = new StampMargin(5, 5)
            });

            AssertSize(new Size(120, 110), new CircularStamp
            {
                Size = new Size(110, 100),
                Margin = new StampMargin(5, 5)
            });

            AssertSize(new Size(130, 110), new CircularStamp
            {
                Size = new Size(110, 100),
                Margin = new StampMargin(5, 10)
            });

            AssertSize(new Size(110, 120), new CircularStamp
            {
                Size = new Size(100, 110),
                Margin = new StampMargin(5, 5)
            });

            AssertSize(new Size(110, 130), new CircularStamp
            {
                Size = new Size(100, 110),
                Margin = new StampMargin(10, 5)
            });
        }

        [TestMethod]
        public void TestSize_RectangleStamp()
        {
            AssertSize(new Size(110, 110), new RectangleStamp
            {
                Size = new Size(100, 100),
                Margin = new StampMargin(5, 5)
            });

            AssertSize(new Size(120, 110), new RectangleStamp
            {
                Size = new Size(110, 100),
                Margin = new StampMargin(5, 5)
            });

            AssertSize(new Size(130, 110), new RectangleStamp
            {
                Size = new Size(110, 100),
                Margin = new StampMargin(5, 10)
            });

            AssertSize(new Size(110, 120), new RectangleStamp
            {
                Size = new Size(100, 110),
                Margin = new StampMargin(5, 5)
            });

            AssertSize(new Size(110, 130), new RectangleStamp
            {
                Size = new Size(100, 110),
                Margin = new StampMargin(10, 5)
            });
        }



    }
}
