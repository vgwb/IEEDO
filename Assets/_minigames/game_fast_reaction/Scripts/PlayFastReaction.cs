using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;
using vgwb.framework;

namespace minigame.fast_reaction
{
    public class PlayFastReaction : SingletonMonoBehaviour<PlayFastReaction>, MinigameManager
    {
        public enum States
        {
            Init,
            Countdown,
            Play,
            End
        }

        [Header("References")]
        public ImageDisplay Display;
        public ui_timer UI_Timer;
        public ui_score UI_Score;

        [Header("Game Settings")]
        public int PercentSameSymbol;

        private int score = 0;
        private int currentImage = -1;
        private StateMachine<States, StateDriverUnity> fsm;

        protected override void Init()
        {
            fsm = new StateMachine<States, StateDriverUnity>(this);
        }

        public void InitGame()
        {
            fsm.ChangeState(States.Init);
        }

        // void Update()
        // {
        //     fsm.Driver.Update.Invoke();
        // }

        void Init_Enter()
        {
            score = 0;
            currentImage = 0;
            Display.ShowImage(currentImage);
            Debug.Log("Waiting for start button to be pressed");
        }

        void OnPressStart()
        {
            fsm.ChangeState(States.Countdown);
        }

        //We can return a coroutine, this is useful animations and the like
        IEnumerator Countdown_Enter()
        {

            Debug.Log("Starting in 3...");
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Starting in 2...");
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Starting in 1...");
            yield return new WaitForSeconds(0.5f);

            fsm.ChangeState(States.Play);
        }

        void Countdown_OnGUI()
        {
            GUILayout.Label("Look at Console");
        }

        void Play_Enter()
        {
            Debug.Log("FIGHT!");
        }

        void Play_Update()
        {

            if (UI_Timer.timeRemaining <= 0)
            {
                fsm.ChangeState(States.End);
            }
        }

        void Play_Exit()
        {
            Debug.Log("Game Over");
        }

        void End_Enter()
        {
            Debug.Log("End Game");
        }

        public void OnBtnYes()
        {
            CheckAnswer(true);
        }

        public void OnBtnNo()
        {
            CheckAnswer(false);
        }

        void CheckAnswer(bool choice)
        {
            SelectNewImage();

            if (choice)
            {
                score++;
                UI_Score.AddScore(1, score);
            }
        }

        void SelectNewImage()
        {
            if (Random.Range(1, 100) < PercentSameSymbol)
            {
                // same image
            }
            else
            {
                currentImage = Random.Range(0, Display.GetAlbumSize());
            }
            Display.ShowImage(currentImage);
        }
    }
}
