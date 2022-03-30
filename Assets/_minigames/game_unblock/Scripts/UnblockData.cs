using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace minigame.unblock
{
    public class UnblockData : ScriptableObject
    {
        [HideInInspector]
        public float frameW, frameH;
        [HideInInspector]
        public int[,] blockState;

        public static UnblockData Instance;
        public static UnblockData I()
        {
            if (Instance == null)
            {
                Instance = CreateInstance<UnblockData>();
            }
            return Instance;
        }

        public void resetBlocks()
        {
            blockState = new int[GameData.I().blockSizex, GameData.I().blockSizey];
        }

        /// <summary>
        /// Sets the state of the block.
        /// </summary>
        /// <param name="blockType">Block type.</param>
        /// <param name="tx">Tx.</param>
        /// <param name="ty">Ty.</param>
        /// <param name="state">State.1,occupy 0,not occupy</param>
        public void setBlockState(int blockType, int tx, int ty, int state)
        {
            //clear self block datas first
            //Debug.Log("tx__ty" + tx + "__" + ty);
            blockState[tx, ty] = state;

            switch (blockType)
            {
                case 1:
                    blockState[tx + 1, ty] = state;
                    break;
                case 2:
                    blockState[tx + 1, ty] = state;
                    break;
                case 3:
                    blockState[tx, ty + 1] = state;
                    break;
                case 4:
                    blockState[tx + 1, ty] = state;
                    blockState[tx + 2, ty] = state;
                    break;
                case 5:
                    blockState[tx, ty + 1] = state;
                    blockState[tx, ty + 2] = state;
                    break;
            }
        }

        public int getleft(int type, int tx, int ty)
        {
            int place = 0;//destintion grid x
            switch (type)
            {
                case 1:
                    //check its left until 0(left border)
                    for (int i = tx; i >= 0; i--)
                    {
                        place = i;
                        if (i > 0)
                        {
                            if (blockState[i - 1, ty] == 1)
                                break;
                        }
                        else
                        {
                            break;
                        }
                    }

                    break;
                case 2:
                    for (int i = tx; i >= 0; i--)
                    {
                        place = i;
                        if (i > 0)
                        {
                            if (blockState[i - 1, ty] == 1)
                                break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                case 3://if block can only move up and down,here mean the bottom range block can move
                    for (int i = ty; i >= 0; i--)
                    {
                        place = i;
                        if (i > 0)
                        {
                            if (blockState[tx, i - 1] == 1)
                                break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                case 4:
                    for (int i = tx; i >= 0; i--)
                    {
                        place = i;
                        if (i > 0)
                        {
                            if (blockState[i - 1, ty] == 1)
                                break;
                        }
                    }
                    break;
                case 5:
                    for (int i = ty; i >= 0; i--)
                    {
                        place = i;
                        if (i > 0)
                        {
                            if (blockState[tx, i - 1] == 1)
                                break;
                        }
                    }
                    break;
            }
            return place;
        }

        public int getright(int type, int tx, int ty)
        {
            int place = 0;
            switch (type)
            {
                case 1:
                    for (int i = tx; i <= GameData.instance.blockSizex - 2; i++)
                    {
                        place = i;
                        if (i < GameData.instance.blockSizex - 2)
                        {
                            if (blockState[i + 2, ty] == 1)
                                break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                case 2:
                    for (int i = tx; i <= GameData.instance.blockSizex - 2; i++)
                    {
                        place = i;

                        if (i < GameData.instance.blockSizex - 2)
                        {
                            if (blockState[i + 2, ty] == 1)
                                break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                case 3:
                    for (int i = ty; i <= GameData.instance.blockSizey - 2; i++)
                    {
                        place = i;

                        if (i < GameData.instance.blockSizey - 2)
                        {
                            if (blockState[tx, i + 2] == 1)
                                break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                case 4:
                    for (int i = tx; i <= GameData.instance.blockSizex - 3; i++)
                    {
                        place = i;
                        if (i < GameData.instance.blockSizex - 3)
                        {
                            if (blockState[i + 3, ty] == 1)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                case 5:
                    for (int i = ty; i <= GameData.instance.blockSizey - 3; i++)
                    {
                        place = i;
                        if (i < GameData.instance.blockSizex - 3)
                        {
                            if (blockState[tx, i + 3] == 1)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
            }
            return place;
        }
    }
}
