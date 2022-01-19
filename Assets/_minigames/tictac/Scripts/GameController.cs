using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Ieedo.games.tictac
{
    public class GameController : MonoBehaviour
    {

        public AIScript ai;
        public TurnStateImageScript turnStateImage;
        public WinLineScript winLine;
        public GameObject menuScreen, gameScreen, gameOverScreen;
        public RectTransform protectFromInputPanel;
        public FieldScript field;
        public TMPro.TextMeshProUGUI winText;
        public string winTextStr, tieTextStr;

        public Color xColor, oColor;

        public static GameController controller;

        private Cell[] cellList;
        private Cell[,] cellMatrix;

        private Cell lastClickedCell;

        private int turnsCount = 0;

        private PlayerSymbol curPlayerSymbol = PlayerSymbol.X;

        private GameState curGameState = GameState.MENU;

        private GameMode gameMode = GameMode.MULTIPLAYER;

        public enum GameState { RESUME, MENU, GAME_OVER }
        public enum PlayerSymbol { X, O }
        public enum GameMode { SINGLEPLAYER, MULTIPLAYER }
        public enum LineType { HORIZONTAL, VERTICAL, DIAGONAL_M, DIAGONAL_S }

        public PlayerSymbol CurPlayerSymbol
        {
            get { return curPlayerSymbol; }
        }

        public GameState CurGameState
        {
            set { curGameState = value; }

            get { return curGameState; }
        }

        private void Awake()
        {
            controller = this;
        }

        // Use this for initialization
        void Start()
        {
            cellList = field.GetCells();
            cellMatrix = new Cell[3, 3];

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    cellMatrix[i, j] = cellList[i * 3 + j];
                }
            }

            SetGameOnMenu();
        }

        public void UpdateTurn(Cell lastCell)
        {
            turnsCount++;

            lastClickedCell = lastCell;

            if (turnsCount >= 5) CheckForWin();

            if (CurGameState != GameState.GAME_OVER) {
                SwitchCurPlayer();
            }
        }

        private void SwitchCurPlayer()
        {
            if (curPlayerSymbol == PlayerSymbol.X) curPlayerSymbol = PlayerSymbol.O;
            else curPlayerSymbol = PlayerSymbol.X;

            if (gameMode == GameMode.SINGLEPLAYER && curPlayerSymbol == ai.symbol)
                AIturn();
            else ProtectFromInput(false);

            turnStateImage.UpdateImage();
        }

        /// <summary>
        /// Checks for win conditions and changes game state. If someone wins - generates a win line.
        /// </summary>
        private void CheckForWin()
        {
            int xRowScore = 0, oRowScore = 0, xColScore = 0, oColScore = 0;
            int xMainDiagScore = 0, oMainDiagScore = 0, xSecDiagScore = 0, oSecDiagScore = 0;

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    //rows
                    if (cellMatrix[i, j].CurCellState == Cell.CellState.X) xRowScore++;
                    else if (cellMatrix[i, j].CurCellState == Cell.CellState.O) oRowScore++;

                    //columns
                    if (cellMatrix[j, i].CurCellState == Cell.CellState.X) xColScore++;
                    else if (cellMatrix[j, i].CurCellState == Cell.CellState.O) oColScore++;

                    //main diag
                    if (i == j) {
                        if (cellMatrix[i, j].CurCellState == Cell.CellState.X) xMainDiagScore++;
                        else if (cellMatrix[i, j].CurCellState == Cell.CellState.O) oMainDiagScore++;
                    }

                    //secondary diag
                    if (i + j == 2) {
                        if (cellMatrix[i, j].CurCellState == Cell.CellState.X) xSecDiagScore++;
                        else if (cellMatrix[i, j].CurCellState == Cell.CellState.O) oSecDiagScore++;
                    }

                }//end of the inner loop

                if (xRowScore == 3 || xColScore == 3 || xMainDiagScore == 3 || xSecDiagScore == 3) {
                    SetGameOnGameOver();

                    winText.text = winTextStr;
                    winText.color = xColor;

                    Debug.Log("X wins");

                    break;
                } else if (oRowScore == 3 || oColScore == 3 || oMainDiagScore == 3 || oSecDiagScore == 3) {
                    SetGameOnGameOver();

                    winText.text = winTextStr;
                    winText.color = oColor;

                    Debug.Log("O wins");

                    break;
                }

                xRowScore = 0;
                oRowScore = 0;
                xColScore = 0;
                oColScore = 0;
            }

            if (curGameState == GameState.GAME_OVER) //If game is over - create a win line.
            {
                LineType lineType;

                if (curPlayerSymbol == PlayerSymbol.X)
                    lineType = CheckWinLineType(xRowScore, xColScore, xMainDiagScore, xSecDiagScore);
                else
                    lineType = CheckWinLineType(oRowScore, oColScore, oMainDiagScore, oSecDiagScore);

                GenerateWinLine(lineType);
            } else if (turnsCount == 9) //If game isn't over and field is full - tie.
              {
                SetGameOnGameOver();
                winLine.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f); //to hide win line when tie

                winText.text = tieTextStr;
                winText.color = Color.white;

                Debug.Log("TIE");
            }

        }

        /// <summary>
        /// Returns a <see cref="LineType"/> type of the win line. Is used during the win line generation.
        /// </summary>
        /// <param name="row">Row score.</param>
        /// <param name="col">Column score.</param>
        /// <param name="d_m">Main diagonal score.</param>
        /// <param name="d_s">Secondary diagonal score.</param>
        /// <returns></returns>
        private LineType CheckWinLineType(int row, int col, int d_m, int d_s)
        {
            if (row == 3) return LineType.HORIZONTAL;
            else if (col == 3) return LineType.VERTICAL;
            else if (d_m == 3) return LineType.DIAGONAL_M;
            else return LineType.DIAGONAL_S;
        }

        /// <summary>
        /// Changes win line parameters to suite win conditions (rotation, size, position, color).
        /// </summary>
        /// <param name="type"></param>
        private void GenerateWinLine(LineType type)
        {
            Cell lineOrigin;

            Int2D lastCellIndex = GetCellMatrixIndex(lastClickedCell);

            switch (type) {
                case LineType.HORIZONTAL:
                    lineOrigin = cellMatrix[lastCellIndex.x, 0];
                    winLine.transform.eulerAngles = Vector3.zero;
                    winLine.SetStraight(true);
                    break;
                case LineType.VERTICAL:
                    lineOrigin = cellMatrix[0, lastCellIndex.y];
                    winLine.transform.eulerAngles = new Vector3(0f, 0f, -90f);
                    winLine.SetStraight(true);
                    break;
                case LineType.DIAGONAL_M:
                    lineOrigin = cellMatrix[0, 0];
                    winLine.transform.eulerAngles = new Vector3(0f, 0f, -45f);
                    winLine.SetStraight(false);
                    break;
                case LineType.DIAGONAL_S:
                    lineOrigin = cellMatrix[2, 0];
                    winLine.transform.eulerAngles = new Vector3(0f, 0f, 45f);
                    winLine.SetStraight(false);
                    break;
                default:
                    lineOrigin = cellList[0];
                    break;
            }

            if (curPlayerSymbol == PlayerSymbol.X) winLine.GetComponent<Image>().color = xColor;
            else winLine.GetComponent<Image>().color = oColor;

            winLine.transform.position = lineOrigin.transform.position;
        }

        public void SetGameOnMenu()
        {
            gameScreen.SetActive(false);
            gameOverScreen.SetActive(false);
            menuScreen.SetActive(true);

            CurGameState = GameState.MENU;
        }

        public void SetGameOnResume()
        {
            menuScreen.SetActive(false);
            gameOverScreen.SetActive(false);
            gameScreen.SetActive(true);

            turnStateImage.gameObject.SetActive(true);

            CurGameState = GameState.RESUME;

            ai.gameObject.SetActive(gameMode == GameMode.SINGLEPLAYER);
            if (ai.gameObject.activeInHierarchy && ai.symbol == curPlayerSymbol) AIturn();
            else ProtectFromInput(false);

            field.AppearAnim();
        }

        public void SetGameOnGameOver()
        {
            menuScreen.SetActive(false);
            gameScreen.SetActive(true);
            gameOverScreen.SetActive(true);

            protectFromInputPanel.gameObject.SetActive(true);

            turnStateImage.gameObject.SetActive(false);

            CurGameState = GameState.GAME_OVER;
        }

        public void SingleplayerMode()
        {
            gameMode = GameMode.SINGLEPLAYER;
        }

        public void MultiplayerMode()
        {
            gameMode = GameMode.MULTIPLAYER;
        }

        public void AIturn()
        {
            ai.ActivateForTurn();
            ProtectFromInput(true);
        }

        /// <summary>
        /// Resets controller values to default ones.
        /// </summary>
        public void ResetController()
        {
            foreach (Cell c in cellList) c.ResetCell();
            curPlayerSymbol = PlayerSymbol.X;
            turnsCount = 0;

            winLine.ResetLine();
            ai.ResetAI();

            lastClickedCell = null;

            turnStateImage.UpdateImage();
        }

        public void ProtectFromInput(bool val)
        {
            protectFromInputPanel.gameObject.SetActive(val);
        }

        public Cell[,] GetCellMatrix()
        {
            return cellMatrix;
        }

        public Cell[] GetCellList()
        {
            return cellList;
        }

        /// <summary>
        /// Returns <see cref="Int2D"/> index of a specified cell in the 2-dimensional array.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public Int2D GetCellMatrixIndex(Cell c)
        {
            Int2D index = new Int2D();

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    if (c == cellMatrix[i, j]) {
                        index.x = i;
                        index.y = j;
                    }
                }
            }

            return index;
        }

        public Cell GetLastClickedCell()
        {
            return lastClickedCell;
        }

        /// <summary>
        /// Is used to simplify operations on 2-dimensional arrays.
        /// </summary>
        public struct Int2D
        {
            public int x, y;

            Int2D(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public static Int2D operator +(Int2D i1, Int2D i2)
            {
                return new Int2D(i1.x + i2.x, i1.y + i2.y);
            }

            public static Int2D operator -(Int2D i1, Int2D i2)
            {
                return new Int2D(i1.x - i2.x, i1.y - i2.y);
            }
        }
    }
}