using System;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine.Localization.Settings;

namespace Ieedo
{
    [Serializable]
    public struct Timestamp
    {
        public Timestamp(DateTime dateTime)
        {
            this.binaryTimestamp = dateTime.ToBinary();
        }

        public long binaryTimestamp;

        public override string ToString()
        {
            return DateTime.FromBinary(binaryTimestamp).ToString("ddd dd MMM", LocalizationSettings.SelectedLocale.Formatter);
        }

        [JsonIgnore]
        public static Timestamp Now => new() { binaryTimestamp = DateTime.Now.ToBinary() };

        [JsonIgnore]
        public static Timestamp None => new() { binaryTimestamp = 0 };

        [JsonIgnore]
        public static Timestamp Today => new() { binaryTimestamp = DateTime.Today.ToBinary() };

        [JsonIgnore]
        public DateTime Date => DateTime.FromBinary(binaryTimestamp);
    }
}
