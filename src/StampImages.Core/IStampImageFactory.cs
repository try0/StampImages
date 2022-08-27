using System.Drawing;

namespace StampImages.Core
{
    public interface IStampImageFactory<IMAGE_OBJECT>
    {
        StampImageFactoryConfig Config { get; set; }

        IMAGE_OBJECT Create(BaseStamp stamp);

        IMAGE_OBJECT Create(CircularStamp stamp);

        IMAGE_OBJECT Create(RectangleStamp stamp);

        IMAGE_OBJECT Create(ThreeAreaCircularStamp stamp);

        IMAGE_OBJECT Resize(IMAGE_OBJECT src, int width, int height);

        void Save(BaseStamp stamp, string fileName);
    }
}