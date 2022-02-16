using System;

namespace Ieedo
{
    [Serializable]
    public struct LocalizedString
    {
        public UnityEngine.Localization.LocalizedString Key;
        public string DefaultText;

        public LocalizedString(string defaultText) : this()
        {
            DefaultText = defaultText;
        }

        public string Text
        {
            get
            {
                if (!Key.IsEmpty) return Key.GetLocalizedString();
                return DefaultText;
            }
        }
    }
}