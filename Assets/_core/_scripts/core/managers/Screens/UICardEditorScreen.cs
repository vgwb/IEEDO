using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Gui;
using UnityEngine;

namespace Ieedo
{
    public class UICardEditorScreen : UIScreen
    {
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
                UICardManager.I.AddCardUI(card, CardPivot);
            }
        }


        void Start()
        {
            //SetupButton(btnDelete, () => GoTo(ScreenID.Assessment));
        }

        protected override IEnumerator OnOpen()
        {
            LoadCurrentCards();
            yield return base.OnOpen();
        }
    }
}