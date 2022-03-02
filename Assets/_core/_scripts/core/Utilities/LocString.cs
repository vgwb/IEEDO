using System;
using UnityEngine.Localization;

namespace Ieedo
{
    [Serializable]
    public struct LocString
    {
        public UnityEngine.Localization.LocalizedString Key;
        public string DefaultText;

        public LocString(string table, string locKey, string defaultText = "") : this()
        {
            Key = new LocalizedString( table,locKey);
            DefaultText = defaultText;
        }

        public string Text
        {
            get
            {
                if (Key != null && !Key.IsEmpty) return Key.GetLocalizedString();
                return DefaultText;
            }
        }
    }
}
