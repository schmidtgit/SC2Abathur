using NydusNetwork.API.Protocol;
using System;

namespace Abathur.Core.Intel.Map
{
    public class ImageDataHandler {
        private byte[] _data;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public ImageDataHandler(ImageData img) : this(img.Size.X, img.Size.Y, img.Data.ToByteArray()) {
            if(img.BitsPerPixel != 8)
                throw new ArgumentException();
        }

        public ImageDataHandler(int width, int height, byte[] data) {
            _data = data;
            Width = width;
            Height = height;
        }

        public void UpdateImage(ImageData img) => _data = img.Data.ToByteArray();

        public void Set(int x,int y, byte value = 0) {
            if(CalculateIndex(x,y,out int index))
                _data[index] = value;
        }

        public bool IsSet(int x,int y) {
            if(CalculateIndex(x,y,out int index))
                return _data[index] != 0;
            return false;
        }

        public int GetNumber(int x,int y)
        {
            if (CalculateIndex(x,y,out int index))
            {
                return _data[index];
            }
            return 0;
        }

        private bool CalculateIndex(int x,int y, out int index) {
            if(x == 0 || y == 0) { index = 0; return false; }
            index = x + ((Height - 1) - y) * Width;
            if(index > _data.Length)
                return false;
            return true;
        }
    }
}