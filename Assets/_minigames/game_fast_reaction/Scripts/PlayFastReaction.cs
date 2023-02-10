using UnityEngine;
using UnityEngine.UI;
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
        public UIButton ButtonYes;
        public UIButton ButtonNo;
        public UIButton ButtonStart;
        public ui_timer UI_Timer;
        public ui_score UI_Score;
        public ui_score UI_Combo;
        public ui_countdown UI_Countdown;

        [Header("Game Settings")]
        public int BaseScore = 50;
        public int SameSymbolProbability = 60;
        public int TimerDuration = 60;

        private int level = 1;
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

        void Update()
        {
            fsm.Driver.Update.Invoke();
        }

        #region FSM
        void Init_Enter()
        {
            level = 1;
            score = 0;
            combo_counter = 0;
            combo_multiplier = 1;
            currentImage = previousImage = -1;
            UI_Score.Init(score);
            UI_Combo.Init(level);
            UI_Timer.Init(TimerDuration, () => WhenTimerFinishes());
            SelectFirstImage();
            ButtonStart.Show();
            ButtonYes.Hide();
            ButtonNo.Hide();
            UI_Countdown.gameObject.SetActive(false);
        }

        IEnumerator Countdown_Enter()
        {
            ButtonStart.Hide();
            UI_Countdown.gameObject.SetActive(true);
            ButtonYes.Show();
            ButtonNo.Show();

            UI_Countdown.Show(3);
            SoundManager.I.PlaySfx(AudioEnum.game_timer);
            yield return new WaitForSeconds(1f);
            UI_Countdown.Show(2);
            SoundManager.I.PlaySfx(AudioEnum.game_timer);
            yield return new WaitForSeconds(1f);
            UI_Countdown.Show(1);
            SoundManager.I.PlaySfx(AudioEnum.game_timer);
            yield return new WaitForSeconds(1f);
            UI_Countdown.gameObject.SetActive(false);

            fsm.ChangeState(States.Play);
        }

        void Play_Enter()
        {
            UI_Timer.StartTimer();
            SelectNewImage();
        }

        void Play_Update()
        {
        }

        void Play_Exit()
        {
            Debug.Log("Game Over");
        }

        void End_Enter()
        {
            Debug.Log("End Game");
            ActivityFastReaction.I.FinishGame(score, level);
        }

        #endregion

        // callback from Timer
        public void WhenTimerFinishes()
        {
            fsm.ChangeState(States.End);
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
                    UI_Combo.AddScore(1, combo_multiplier);
                }
                UI_Score.AddScore(delta_score, score);
            }
            else
            {
                combo_counter = 0;
                UI_Combo.AddScore(1 - combo_multiplier, 1);
                combo_multiplier = 1;
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
            if (fsm.State == States.Play)
            {
                CheckAnswer(true);
            }
        }

        public void OnBtnNo()
        {
            if (fsm.State == States.Play)
            {
                CheckAnswer(false);
            }
        }

    }
}
