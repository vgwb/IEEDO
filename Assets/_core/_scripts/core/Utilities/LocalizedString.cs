using System;

namespace Ieedo
{
    [Serializable]
    public struct LocalizedString
    {
        public string key;
        public string DefaultText;

        public LocalizedString(string defaultText) : this()
        {
            DefaultText = defaultText;
        }

        public string Text => DefaultText;  // TODO: this should come from localization, but may be defined here for ease
    }
}