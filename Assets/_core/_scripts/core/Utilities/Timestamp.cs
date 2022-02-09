using System;
using System.Globalization;

namespace Ieedo
{
    [Serializable]
    public struct Timestamp
    {
        public long binaryTimestamp;
        public static Timestamp Now => new() {binaryTimestamp = DateTime.Now.ToBinary()};

        public override string ToString()
        {
            return DateTime.FromBinary(binaryTimestamp).ToString("ddd dd MMM", CultureInfo.CurrentUICulture);
        }

        public DateTime Date => DateTime.FromBinary(binaryTimestamp);
    }
}