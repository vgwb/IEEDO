using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;
using vgwb.framework;
using Ieedo;

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
        public GameObject BtnStart;
        public ui_countdown UI_Countdown;

        [Header("Game Settings")]
        public int BaseScore = 50;
        public int SameSymbolProbability = 60;
        public int TimerDuration = 60;

        private int level = 0;
        private int score = 0;
        private int delta_score = 0;
        private int combo_multiplier = 1;
        private int combo_counter = 0;
        private int currentImage = -1;
        private int previousImage = -1;
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
            level = 0;
            score = 0;
            combo_counter = 0;
            combo_multiplier = 1;
            currentImage = previousImage = -1;
            UI_Timer.Init(TimerDuration);
            SelectFirstImage();
            BtnStart.SetActive(true);
            UI_Countdown.gameObject.SetActive(false);
        }

        IEnumerator Countdown_Enter()
        {
            BtnStart.SetActive(false);
            UI_Countdown.gameObject.SetActive(true);

            UI_Countdown.Show(3);
            yield return new WaitForSeconds(1f);
            UI_Countdown.Show(2);
            yield return new WaitForSeconds(1f);
            UI_Countdown.Show(1);
            yield return new WaitForSeconds(1f);
            UI_Countdown.gameObject.SetActive(false);

            fsm.ChangeState(States.Play);
        }

        void Play_Enter()
        {
            UI_Timer.StartTimer(TimerDuration);
            SelectNewImage();
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

        void CheckAnswer(bool choice)
        {

            if ((choice && previousImage == currentImage) || (!choice && previousImage != currentImage))
            {
                level++;
                delta_score = BaseScore * combo_multiplier;
                score += delta_score;
                combo_counter++;
                if (combo_counter > 5)
                {
                    combo_counter = 0;
                    combo_multiplier++;
                    SoundManager.I.PlaySfx(AudioEnum.game_win);
                }
                UI_Score.AddScore(delta_score, score);
            }
            else
            {
                combo_multiplier = 1;
                combo_counter = 0;
                SoundManager.I.PlaySfx(AudioEnum.game_error);
            }
            SelectNewImage();
        }

        void SelectNewImage()
        {
            previousImage = currentImage;
            if (Random.Range(1, 100) < SameSymbolProbability)
            {
                // same image
            }
            else
            {
                currentImage = Random.Range(0, Display.GetAlbumSize());
            }
            Display.NewImage(currentImage);
        }

        void SelectFirstImage()
        {
            currentImage = Random.Range(0, Display.GetAlbumSize());
            previousImage = currentImage;
            Display.ShowImage(currentImage);
        }

        public void OnBtnStart()
        {
            fsm.ChangeState(States.Countdown);
        }

        public void OnBtnYes()
        {
            CheckAnswer(true);
        }

        public void OnBtnNo()
        {
            CheckAnswer(false);
        }

    }
}
