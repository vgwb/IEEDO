using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace minigame.unblock
{
    public class BlockOnMouseDrag : MonoBehaviour
    {
        float startX = 0f;
        float startY = 0f;
        float myW, myH;
        int type = 0;
        Camera camera1;

        void Start()
        {
            startX = transform.position.x;
            startY = transform.position.y;

            type = int.Parse(name);
            camera1 = transform.root.GetComponent<Camera>();
            if (camera1 == null)
                camera1 = transform.root.GetComponentInChildren<Camera>();
        }

        bool canMove = true;
        int min = 0; int max = 0;
        private Vector3 offset = Vector3.zero;
        int tx, ty;

        void OnMouseDown()
        {
            if (!canMove)
                return;

            if (GameData.I().isLock)
                return;

            Vector3 worldPos = camera1.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = transform.position.z;
            offset = worldPos - transform.position;

            int tx = Mathf.RoundToInt((transform.position.x - GameData.I().startPos.x) / GameData.instance.tileWidth);
            int ty = Mathf.RoundToInt((transform.position.y - GameData.I().startPos.y) / GameData.instance.tileWidth);

            //clear origin data occupy first
            UnblockData.Instance.setBlockState(type, tx, ty, 0);
            //check the max movable position;
            min = UnblockData.Instance.getleft(int.Parse(name), tx, ty);
            max = UnblockData.Instance.getright(int.Parse(name), tx, ty);

        }

        void OnMouseUp()
        {
            if (!canMove)
                return;
            if (GameData.I().isLock)
                return;
            offset = Vector3.zero;

            if (type % 2 == 0 || type == 1)//horizon blocks
            {
                //transform.DOMoveX(Mathf.RoundToInt((transform.position.x - startX) / GameData.instance.tileWidth) * GameData.instance.tileWidth + startX, .2f);
                ATween.MoveTo(gameObject, ATween.Hash("islocal", false, "x", Mathf.RoundToInt((transform.position.x - startX) / GameData.instance.tileWidth) * GameData.instance.tileWidth + startX, "delay", 0f, "easetype", ATween.EaseType.linear, "time", .2f, "OnComplete", (System.Action)(() => { })));
            }
            else//vertical blocks
            {
                //transform.DOMoveY(Mathf.RoundToInt((transform.position.y - startY) / GameData.instance.tileWidth) * GameData.instance.tileWidth + startY, .2f);
                ATween.MoveTo(gameObject, ATween.Hash("islocal", false, "y", Mathf.RoundToInt((transform.position.y - startY) / GameData.instance.tileWidth) * GameData.instance.tileWidth + startY, "delay", 0f, "easetype", ATween.EaseType.linear, "time", .2f, "OnComplete", (System.Action)(() => { })));
            }

            //set the settled block state
            int tx = Mathf.RoundToInt((transform.position.x - GameData.I().startPos.x) / GameData.instance.tileWidth);
            int ty = Mathf.RoundToInt((transform.position.y - GameData.I().startPos.y) / GameData.instance.tileWidth);

            UnblockData.Instance.setBlockState(type, tx, ty, 1);

            if (type == 1)
            {
                if (tx == GameData.instance.exitPos.x - 2 && ty == GameData.instance.exitPos.y)
                {//hero block is on exit,//win;
                    canMove = false;
                    GameData.instance.isLock = true;
                    //transform.DOMoveX(transform.position.x + GameData.instance.tileWidth * 2+1, 2).OnComplete(() =>
                    ATween.MoveTo(gameObject, ATween.Hash("islocal", false, "x", transform.position.x + GameData.instance.tileWidth * 2 + 1, "delay", 0f, "easetype", ATween.EaseType.linear, "time", 2f, "OnComplete", (System.Action)(() =>
                    {
                        ActivityUnblock.I.win();
                    })));
                }
            }

            for (int i = 0; i < GameData.instance.blockSizey; i++)
            {
                string tline = "";
                for (int j = 0; j < GameData.instance.blockSizey; j++)
                {
                    tline += UnblockData.Instance.blockState[j, i];
                }
            }
        }

        void OnMouseDrag()
        {
            if (!canMove)
                return;
            if (GameData.I().isLock)
                return;
            Vector3 point = camera1.ScreenToWorldPoint(Input.mousePosition);

            if (type % 2 == 0 || int.Parse(name) == 1)
            {//for horizon blocks

                point.y = startY;

                float tminx = min * GameData.instance.tileWidth + GameData.I().startPos.x;
                float tmaxx = tminx + (max - min) * GameData.instance.tileWidth;//(BlockOutData.Instance.frameW / 6);


                float minx = Mathf.Min(tminx, tmaxx);
                float maxx = Mathf.Max(tminx, tmaxx);

                point.x = Mathf.Clamp(point.x - offset.x, minx, maxx);
            }
            else//for vertical blocks
            {
                point.x = startX;

                float tmin = Mathf.Min(min, max);
                float tmax = Mathf.Max(min, max);

                float tminy = GameData.I().startPos.y + tmin * GameData.instance.tileWidth;
                float tmaxy = tminy + (tmax - tmin) * GameData.instance.tileWidth;

                float miny2 = Mathf.Min(tminy, tmaxy);
                float maxy2 = Mathf.Max(tminy, tmaxy);

                point.y = Mathf.Clamp(point.y - offset.y, miny2, maxy2);
            }

            transform.position = point;
        }
    }
}
