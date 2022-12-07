using Ieedo.Utilities;
using UnityEngine;

namespace Ieedo
{
    public class UICardManager : SingletonMonoBehaviour<UICardManager>
    {
        public GameObject cardPrefab;

        public UICard CreateCardUI(CardData cardData, Transform parentTr)
        {
            var cardGo = GameObject.Instantiate(cardPrefab, parentTr, false);
            cardGo.transform.localScale = Vector3.one;
            cardGo.transform.localPosition = Vector3.zero;
            var uiCard = cardGo.GetComponent<UICard>();
            cardGo.name = $"Card_{cardData.DefID}";
            uiCard.AssignCard(cardData);
            return uiCard;
        }
    }
}