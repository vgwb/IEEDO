using System.Collections;
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
            GameManager.I().init();
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
            //Debug.Log("initGame Unblock");
            yield return new WaitForEndOfFrame();
            string[] tData = ScriptableObject.CreateInstance<Datas>().getData("unblock");
            var gridContainerOri = GameObject.Find("gridContainerori");
            gridContainer = Instantiate(gridContainerOri, gridContainerOri.transform.parent);
            gridContainer.name = "gridContainer";
            blockframe = GameObject.Find("board");
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

            UnblockData.I().frameW = tw;
            UnblockData.I().frameH = th;
            int tlevel = GameData.instance.cLevel;//20;// UnityEngine.Random.Range(0, 500);

            levelData = JSONArray.Parse(tData[tlevel]);

            LevelEntity le = getPuzzle(tlevel);//get level no;0 =level1 1=level2..
            UnblockData.I().resetBlocks();

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
                tblock = Instantiate(tblock, gridContainer.transform);
                tblock.name = type.ToString();
                tblock.AddComponent<BlockOnMouseDrag>();

                float tblockOriWidth = tblock.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
                int _myGridWidth = myGridWidth[type - 1];
                float tscale = tblockOriWidth / (UnblockData.Instance.frameW * blockframe.transform.localScale.x / GameData.I().blockSizex * _myGridWidth);
                tblock.transform.localScale /= tscale;
                zoomscale = tscale;
                float tcblockWidth = tblock.GetComponent<SpriteRenderer>().bounds.size.x / _myGridWidth;
                GameData.instance.tileWidth = tcblockWidth;
                GameData.instance.startPos = -new Vector3(GameData.instance.tileWidth * GameData.I().blockSizex / 2, GameData.instance.tileWidth * GameData.I().blockSizey / 2, 0) + GameData.instance.cameraOffset;
                tblock.transform.position = new Vector3(tpx, tpy, 0) * tcblockWidth + GameData.instance.startPos;
            }

            var texit = Resources.Load("unblock/sprite/exitarea") as GameObject;
            texit = Instantiate(texit, gridContainer.transform);
            texit.transform.localScale /= zoomscale;
            texit.transform.position = GameData.instance.exitPos * GameData.instance.tileWidth + GameData.instance.startPos;
        }

        public LevelEntity getPuzzle(int levelNo)
        {
            var levelEntity = new LevelEntity();
            GameData.I().blockSizex = levelData["w"];
            GameData.I().blockSizey = levelData["h"];
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
            DestroyImmediate(gridContainer);
            if (restart)
            {
                init();
            }
        }
    }
}
