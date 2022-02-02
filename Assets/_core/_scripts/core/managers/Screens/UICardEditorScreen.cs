using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Gui;
using UnityEngine;

namespace Ieedo
{
    public class UICardEditorScreen : UIScreen
    {
        public GameObject cardPrefab;
        public Transform CardPivot;
        public List<UICard> UICards = new List<UICard>();

        public override ScreenID ID => ScreenID.CardEditor;

        public LeanButton btnAdd;
        //public LeanButton btnDelete;

        public void LoadCurrentCards()
        {
            Statics.Data.LoadCards();

            foreach (var card in Statics.Data.Cards)
            {
                AddCardUI(card);
            }
        }

        private UICard AddCardUI(CardDefinition card)
        {
            var cardGo = Instantiate(cardPrefab, CardPivot, true);
            var uiCard = cardGo.GetComponent<UICard>();
            uiCard.AssignDefinition(card);
            UICards.Add(uiCard);

            uiCard.transform.localEulerAngles = new Vector3(0, 0, -10f);
            return uiCard;
        }

        void Start()
        {
            SetupButton(btnAdd, () =>
            {
                var card = Statics.Cards.GenerateCard(
                    new CardDefinition {
                        Category = (CategoryID)UnityEngine.Random.Range(1, 4),
                        Description = new LocalizedString() { DefaultText = "TEST" + UnityEngine.Random.Range(0, 50) },
                        Difficulty = (uint)UnityEngine.Random.Range(1, 5),
                        Title = new LocalizedString() { DefaultText = "TEST" + UnityEngine.Random.Range(0, 50) },
                    });
                AddCardUI(card);
            });
            //SetupButton(btnDelete, () => GoTo(ScreenID.Assessment));
        }

        protected override IEnumerator OnOpen()
        {
            LoadCurrentCards();
            yield return base.OnOpen();
        }
    }
}