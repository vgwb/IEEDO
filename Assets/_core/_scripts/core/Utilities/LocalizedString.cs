using System;

namespace Ieedo
{
    [Serializable]
    public struct LocalizedString
    {
        public string DefaultText;
        public string Text => DefaultText;  // TODO: this should come from localization, but may be defined here for ease
    }
}