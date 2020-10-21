using NydusNetwork.API.Protocol;
using System;
using System.Collections;

namespace Abathur.Core.Intel.Map {
    public abstract class MapHandler {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public abstract void UpdateImage(ImageData img);
        public abstract void Set(int x, int y, byte value = 0);
        public abstract bool IsSet(int x, int y);
        public abstract int GetValue(int x, int y);
        protected bool CalculateIndex(int x, int y, int l, out int index) {
            index = x + y * Width;
            return index <= l;
        }

        public static MapHandler Instantiate(ImageData img) {
            if (img.BitsPerPixel == 8)
                return new ByteMapHandler { Width = img.Size.X, Height = img.Size.Y, _data = img.Data.ToByteArray() };
            if (img.BitsPerPixel == 1)
                return new BitMapHandler { Width = img.Size.X, Height = img.Size.Y, _data = new BitArray(img.Data.ToByteArray()) };
            throw new ArgumentException();
        }

        public static MapHandler Instantiate(int w, int h)
            => new BitMapHandler { Width = w, Height = h, _data = new BitArray(w * h) };
    }
}
