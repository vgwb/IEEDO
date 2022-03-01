﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using Ieedo.utilities;

namespace minigame.unblock
{
    public class Unblock : MonoBehaviour
    {
        JSONNode levelData;
        int[] myGridWidth = { 2, 2, 1, 3, 1 };

        void Start()
        {
            GameManager.getInstance().init();
        }

        public void init()
        {
            if (transform.childCount > 0)
            {
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }
            }
            StartCoroutine("initGame");
        }
        GameObject gridContainer;
        GameObject blockframe;
        IEnumerator initGame()
        {
            yield return new WaitForEndOfFrame();
            string[] tData = ScriptableObject.CreateInstance<Datas>().getData("unblock");

            var gridContainerOri = GameObject.Find("gridContainerori");
            gridContainer = Instantiate(gridContainerOri, gridContainerOri.transform.parent);
            gridContainer.name = "gridContainer";

            blockframe = GameObject.Find("chessboard");
            var corner4 = new Vector3[4];
            float tw, th;

            GameData.instance.cameraOffset = transform.parent.position;

            if (blockframe != null)
            {

                blockframe.GetComponent<Image>().rectTransform.GetLocalCorners(corner4);
                tw = (corner4[2] - corner4[0]).x;
                th = (corner4[2] - corner4[0]).y;
            }
            else
            {
                blockframe = GameObject.Find("Roundsquare");
                tw = blockframe.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
                th = blockframe.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
            }

            UnblockData.getInstance().frameW = tw;
            UnblockData.getInstance().frameH = th;
            int tlevel = GameData.instance.cLevel;//20;// UnityEngine.Random.Range(0, 500);

            levelData = JSONArray.Parse(tData[tlevel]);

            LevelEntity le = getPuzzle(tlevel);//get level no;0 =level1 1=level2..
            UnblockData.getInstance().resetBlocks();

            float zoomscale = 1;
            for (int i = 0; i < le.pieces.Count; i++)
            {
                int tpx = le.pieces[i]._x;
                int tpy = le.pieces[i]._y;
                int tpw = le.pieces[i]._w;
                int tph = le.pieces[i]._h;

                int type = 2;
                if (tpw == 2 && tph == 1)
                {
                    if (i == 0)
                    {
                        type = 1;
                    }
                    else
                    {
                        type = 2;
                    }
                }
                else if (tpw == 1 && tph == 2)
                {
                    type = 3;
                }
                else if (tpw == 3 && tph == 1)
                {
                    type = 4;
                }
                else if (tpw == 1 && tph == 3)
                {
                    type = 5;
                }

                //write block occupy data;
                UnblockData.Instance.setBlockState(type, tpx, tpy, 1);

                var tblock = Resources.Load("unblock/sprite/blocks" + type) as GameObject;
                tblock = Instantiate(tblock, gridContainer.transform) as GameObject;

                tblock.name = type.ToString();
                tblock.AddComponent<BlockOnMouseDrag>();

                float tblockOriWidth = tblock.GetComponent<SpriteRenderer>().sprite.bounds.size.x;

                int _myGridWidth = myGridWidth[type - 1];
                float tscale = (tblockOriWidth / (UnblockData.Instance.frameW * blockframe.transform.localScale.x / GameData.getInstance().blockSizex * _myGridWidth));
                tblock.transform.localScale /= tscale;
                zoomscale = tscale;
                float tcblockWidth = tblock.GetComponent<SpriteRenderer>().bounds.size.x / _myGridWidth;
                GameData.instance.tileWidth = tcblockWidth;

                GameData.instance.startPos = -new Vector3(GameData.instance.tileWidth * GameData.getInstance().blockSizex / 2, GameData.instance.tileWidth * GameData.getInstance().blockSizey / 2, 0) + GameData.instance.cameraOffset;


                tblock.transform.position = new Vector3(tpx, tpy, 0) * tcblockWidth + GameData.instance.startPos;// - new Vector3(GameData.instance.tileWidth* GameData.getInstance().blockSizex / 2 , GameData.instance.tileWidth * GameData.getInstance().blockSizey/ 2,0);
                //tblock.transform.localPosition = new Vector3(tblock.transform.localPosition.x, tblock.transform.localPosition.y, 0);
            }

            GameObject texit = Resources.Load("unblock/sprite/exitarea") as GameObject;
            texit = Instantiate(texit, gridContainer.transform) as GameObject;
            texit.transform.localScale /= zoomscale;
            texit.transform.position = GameData.instance.exitPos * GameData.instance.tileWidth + GameData.instance.startPos;// - new Vector3(GameData.instance.tileWidth * GameData.getInstance().blockSizex / 2, GameData.instance.tileWidth * GameData.getInstance().blockSizey / 2,0);
        }

        public LevelEntity getPuzzle(int levelNo)
        {
            LevelEntity levelEntity = new LevelEntity();

            if (GameData.instance.isTesting)
            {
                levelData = GameData.instance.testData;//this is used for somewhere else not in the game.

            }

            GameData.getInstance().blockSizex = levelData["w"];
            GameData.getInstance().blockSizey = levelData["h"];

            JSONNode tarr = levelData["b"];
            GameData.instance.exitPos = new Vector2(levelData["e"]["x"], levelData["e"]["y"]);

            for (int i = 0; i < tarr.Count; i++)
            {
                var tp = new Piece();
                tp._x = tarr[i]["x"];
                tp._y = tarr[i]["y"];
                tp._w = tarr[i]["w"];
                tp._h = tarr[i]["h"];
                levelEntity.pieces.Add(tp);
            }
            return levelEntity;
        }

        public void clear(bool restart = false)
        {
            //            Debug.Log("Clear");
            DestroyImmediate(gridContainer);
            if (restart)
            {
                init();
            }
        }
    }
}
