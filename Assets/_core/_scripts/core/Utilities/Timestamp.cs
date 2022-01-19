using System;

namespace Ieedo
{
    [Serializable]
    public struct Timestamp
    {
        public long binaryTimestamp;
        public static Timestamp Now => new() {binaryTimestamp = DateTime.Now.ToBinary()};
    }
}