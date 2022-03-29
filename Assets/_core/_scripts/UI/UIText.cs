using UnityEngine;
using TMPro;
using UnityEngine.Localization.Components;

namespace Ieedo
{
    public class UIText : TextMeshProUGUI
    {
        public LocalizeStringEvent LocalizeStringEvent => GetComponent<LocalizeStringEvent>();
        public UnityEngine.Localization.LocalizedString Key
        {
            get => LocalizeStringEvent.StringReference;
            set => LocalizeStringEvent.StringReference = value;
        }
    }
}
