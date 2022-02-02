using Ieedo.Utilities;
using UnityEngine;

namespace Ieedo
{
    public class UICardManager : SingletonMonoBehaviour<UICardManager>
    {
        public GameObject cardPrefab;

        public UICard AddCardUI(CardDefinition card, Transform parentTr)
        {
            var cardGo = GameObject.Instantiate(cardPrefab, parentTr, false);
            cardGo.transform.localScale = Vector3.one;
            cardGo.transform.localPosition = Vector3.zero;
            var uiCard = cardGo.GetComponent<UICard>();
            cardGo.name = $"Card_{card.UID}";
            uiCard.AssignDefinition(card);
            return uiCard;
        }
    }
}