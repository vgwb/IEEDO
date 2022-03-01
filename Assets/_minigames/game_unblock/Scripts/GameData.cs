using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ieedo.utilities;

namespace minigame.unblock
{
    public class GameData : ScriptableObject
    {
        public int nLink = 0; //check in game.When nlink = 0.All the lines linked,so win.
        public int levelPassed = 0;//how much level you passed
        public int cLevel = 0;//currect level
        public int bestScore = 0;//bestscore for level
        public static bool isTrial;//not used
        public static string lastWindow = "";//not used

        public int currentScene = 0;

        public static int[] totalLevel = { 50, 50, 50, 50, 50 };//total levels,currently,we make things easier,only use 50 levels the same for each difficulty
        public List<List<int>> levelStates;
        public static int difficulty = 0;//easy,medium,avanced,hard,expert
        public int mode = 0;

        public bool isTesting = false;
        public JSONNode testData;//for testGame level data;

        public List<int> levelPass;

        public static GameData instance;
        public static GameData getInstance()
        {
            if (instance == null)
            {
                instance = CreateInstance<GameData>();
            }
            return instance;
        }

        public bool isWin = false;//check if win
        public bool isOver = false;//is last level
        public bool isLock = false;//check if game ui can touch or lock and wait
        public string tickStartTime = "0";//game count down.
        public List<int> lvStar = new List<int>();//level stars you got for each level
        public bool isfail = false;//whether the game failed

        /// <summary>
        /// Always uses for initial or reset to start a new level.
        /// </summary>
        public void resetData()
        {
            isLock = false;
            isWin = false;
            isfail = false;
            isOver = false;
            //		levelPassed = PlayerPrefs.GetInt ("levelPass", 0);
            //		Debug.Log ("levelpassed=" + levelPassed);

            tickStartTime = PlayerPrefs.GetString("tipStart", "0");

            //cLevel = 14;//test,set a level number if you want to test directly
            string tData = ScriptableObject.CreateInstance<Datas>().getData("unblock")[cLevel];

            levelData = JSONArray.Parse(tData);
            levelInfo = levelData["s"];


            //get gridSize first;
            gridSizeX = levelData["w"];
            gridSizeY = levelData["h"];

            gridStates = new int[gridSizeX, gridSizeY];
            gridInfo = new JSONNode[gridSizeX, gridSizeY];
        }

        JSONNode levelData;

        public string getLevel(int no)
        {
            return levelData[1]["levels"][no];
        }

        public int gridSizeX = 10;
        public int gridSizeY = 10;

        public Vector2 cheesBoardSize;
        public Vector3[] cheesBoardCorners;
        public float frameWidth;//gameframe width;
        public float frameHeight;//gameframe height;

        public int blockSizex = 6;
        public int blockSizey = 6;

        public float tileWidth = 0;
        public Vector3 startPos;
        public Vector3 exitPos;
        public Vector3 cameraOffset = Vector3.zero;

        public Color[] colors = { Color.clear, Color.red, Color.blue, Color.magenta, Color.cyan, Color.green, Color.yellow, Color.gray, new Color(.8f, .8f, .8f), Color.black, new Color(252f / 255f, 157f / 255f, 154f / 255f), new Color(249f / 255f, 205f / 255f, 173f / 255f), new Color(200f / 255f, 200f / 255f, 169f / 255f) };
        public JSONNode levelInfo;
        public int[,] gridStates;//store color and special color
        public JSONNode[,] gridInfo;//store special grid info
        public int nCorrect = 0;

        public int coin;
    }
}
