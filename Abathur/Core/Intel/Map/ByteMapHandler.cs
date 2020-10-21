using NydusNetwork.API.Protocol;

namespace Abathur.Core.Intel.Map {
    public class ByteMapHandler : MapHandler {
        internal byte[] _data;
        public override void UpdateImage(ImageData img) => _data = img.Data.ToByteArray();

        public override void Set(int x, int y, byte value = 0) {
            if (CalculateIndex(x, y, _data.Length, out int index))
                _data[index] = value;
        }

        public override bool IsSet(int x, int y) {
            if (CalculateIndex(x, y, _data.Length, out int index))
                return _data[index] != 0;
            return false;
        }

        public override int GetValue(int x, int y) {
            if (CalculateIndex(x, y, _data.Length, out int index))
                return _data[index];
            return 0;
        }
    }
}