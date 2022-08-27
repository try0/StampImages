using Microsoft.VisualStudio.TestTools.UnitTesting;
using StampImages.Core;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace StampImages.Test.Net5
{
    [TestClass]
    public class StampTests
    {

        [TestMethod]
        public void TestSetFontFamily_RectangleStamp()
        {
            var stamp = new RectangleStamp
            {
                Text = new StampText("Text")
            };
            stamp.SetFontFamily("Meiryo UI");

            Assert.AreEqual("Meiryo UI", stamp.Text.FontFamily);
        }

        [TestMethod]
        public void TestSetFontFamily_CircularStamp()
        {
            var stamp = new CircularStamp
            {
                Text = new StampText("Text")
            };
            stamp.SetFontFamily("Meiryo UI");

            Assert.AreEqual("Meiryo UI", stamp.Text.FontFamily);
        }

        [TestMethod]
        public void TestSetFontFamily_ThreeAreaCircularStamp()
        {
            var stamp = new ThreeAreaCircularStamp
            {
                TopText = new StampText("TopText"),
                MiddleText = new StampText("MiddleText"),
                BottomText = new StampText("BottomText")
            };
            stamp.SetFontFamily("Meiryo UI");

            Assert.AreEqual("Meiryo UI", stamp.TopText.FontFamily);
            Assert.AreEqual("Meiryo UI", stamp.MiddleText.FontFamily);
            Assert.AreEqual("Meiryo UI", stamp.BottomText.FontFamily);
        }


    }
}
