using System;
using Lean.Gui;
using UnityEngine;

namespace Ieedo
{
    public class UIBottomScreen : UIScreen
    {
        public override ScreenID ID => ScreenID.Bottom;

        public LeanButton btnActivities;
        public LeanButton btnPillars;
        public LeanButton btnCards;

        public Action OnCardsClicked;

        private float cooldown = 0f;
        private bool onCooldown => cooldown > 0f;
        private void StartCooldown()
        {
            cooldown = 0.35f;
        }
        private void Update()
        {
            if (cooldown > 0f) cooldown -= Time.deltaTime;
        }

        void Start()
        {
            SetupButton(btnActivities, () =>
            {
                if (Statics.Input.IsExecutingAction) return;
                if (onCooldown) return;
                StartCooldown();
                Statics.Screens.GoTo(ScreenID.Activities);
            });
            SetupButton(btnPillars, () =>
            {
                if (Statics.Input.IsExecutingAction) return;
                if (onCooldown) return;
                StartCooldown();
                Statics.Screens.GoTo(ScreenID.Pillars);
            });
            SetupButton(btnCards, () =>
            {
                if (Statics.Input.IsExecutingAction) return;
                if (onCooldown) return;
                StartCooldown();
                OnCardsClicked?.Invoke();
                Statics.Screens.GoToTodoList();
            });

            ToggleSelection(btnPillars);
        }

        public void ToggleSelection(LeanButton selectedBtn)
        {
            btnActivities.interactable = btnActivities != selectedBtn;
            btnCards.interactable = btnCards != selectedBtn;
            btnPillars.interactable = btnPillars != selectedBtn;
        }
    }
}
