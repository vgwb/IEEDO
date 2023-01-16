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
            Win,
            Lose
        }

        public float health = 100;
        public float damage = 20;

        private float startHealth;

        private StateMachine<States, StateDriverUnity> fsm;

        private void Awake()
        {
            base.Awake();
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

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(50, 250, 220, 340));

            fsm.Driver.OnGUI.Invoke();

            GUILayout.EndArea();
        }

        void Init_Enter()
        {
            startHealth = health;

            Debug.Log("Waiting for start button to be pressed");
        }

        void Init_OnGUI()
        {
            if (GUILayout.Button("Start"))
            {
                fsm.ChangeState(States.Countdown);
            }
        }

        //We can return a coroutine, this is useful animations and the like
        IEnumerator Countdown_Enter()
        {
            health = startHealth;

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
            health -= damage * Time.deltaTime;

            if (health < 0)
            {
                fsm.ChangeState(States.Lose);
            }
        }

        void Play_OnGUI()
        {
            if (GUILayout.Button("Make me win!"))
            {
                fsm.ChangeState(States.Win);
            }

            GUILayout.Label("Health: " + Mathf.Round(health).ToString());
        }

        void Play_Exit()
        {
            Debug.Log("Game Over");
        }

        void Lose_Enter()
        {
            Debug.Log("Lost");
        }

        void Lose_OnGUI()
        {
            if (GUILayout.Button("Play Again"))
            {
                fsm.ChangeState(States.Countdown);
            }
        }

        void Win_Enter()
        {
            Debug.Log("Won");
        }

        void Win_OnGUI()
        {
            if (GUILayout.Button("Play Again"))
            {
                fsm.ChangeState(States.Countdown);
            }
        }

        public void OnBtnYes()
        {
            Debug.Log("YES");
        }

        public void OnBtnNo()
        {
            Debug.Log("NO");
        }
    }
}
