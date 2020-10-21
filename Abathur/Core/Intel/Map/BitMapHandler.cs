using NydusNetwork.API.Protocol;
using System.Collections;

namespace Abathur.Core.Intel.Map {
    public class BitMapHandler : MapHandler {
        internal BitArray _data;

        public override void UpdateImage(ImageData img) => _data = new BitArray(img.Data.ToByteArray());

        public override void Set(int x, int y, byte value = 0) {
            if (CalculateIndex(x, y, _data.Length, out int index))
                _data.Set(index, value != 0);
        }

        public override bool IsSet(int x, int y) {
            if (CalculateIndex(x, y, _data.Length, out int index))
                return _data[index];
            return false;
        }

        public override int GetValue(int x, int y) {
            if (CalculateIndex(x, y, _data.Length, out int index))
                return _data[index] ? 1 : 0;
            return 0;
        }
    }
}