using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ieedo.games.connect4
{
    public class GameManager : MonoBehaviour
    {
        public game_connect4 GameController;
        public GameObject PawnContainer;
        public bool selectedPlayer1 = true;
        public int difficulty = 0; // 0 - easy, 1- medium,  2 - hard

        public bool player1Turn = true;
        public bool gameOver = false;
        private bool playerWin = false;
        public int[,] boardState; // 0 - empty, 1 - player1, 2 - player2
        private GameObject[,] objects;
        private int lastRow = 0;
        public const int row_count = 6;
        public const int column_count = 7;

        public GameObject player1;
        public GameObject player2;

        public Material color1, winColor1, color2, winColor2;
        public GameObject player1Ghost;
        public GameObject player2Ghost;
        public GameObject fallingPiece;
        public GameObject[] spawnLocations;


        void Start()
        {
            playerWin = false;
            selectedPlayer1 = GameInfo.getSelectedPlayer1();
            difficulty = GameInfo.getDifficulty();

            if (selectedPlayer1)
                Debug.Log("Selected player 1");
            else
                Debug.Log("Selected player 2");
            Debug.Log("Selected difficulty " + difficulty);

            boardState = new int[row_count, column_count];
            player1Turn = true;
            gameOver = false;
            objects = new GameObject[row_count, column_count];
            player2Ghost.SetActive(false);
            player1Ghost.transform.position = spawnLocations[3].transform.position;
            player1Ghost.SetActive(true);
            if (!selectedPlayer1)
            {
                StartCoroutine(aiTurn());
            }
        }

        public void restartScene()
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            GameController.FinishGame(playerWin);
        }

        public void exitToMenu()
        {
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            GameController.FinishGame(playerWin);
        }

        public IEnumerator aiTurn()
        {
            Debug.Log("AI turn");
            yield return new WaitForSeconds(0.7f);
            double minimaxScore;
            int bestColumn;
            switch (difficulty)
            {
                case 0:
                    (bestColumn, minimaxScore) = minimax(boardState, 1, double.NegativeInfinity, double.PositiveInfinity, true);
                    break;
                case 1:
                    (bestColumn, minimaxScore) = minimax(boardState, 3, double.NegativeInfinity, double.PositiveInfinity, true);
                    break;
                case 2:
                    (bestColumn, minimaxScore) = minimax(boardState, 7, double.NegativeInfinity, double.PositiveInfinity, true);
                    break;
                default:
                    (bestColumn, minimaxScore) = minimax(boardState, 1, double.NegativeInfinity, double.PositiveInfinity, true);
                    break;
            }
            TakeTurn(bestColumn);
        }

        public void hoverColumn(int column)
        {
            if (boardState[0, column] == 0 && !gameOver && player1Turn == selectedPlayer1 && (fallingPiece == null || fallingPiece.GetComponent<Rigidbody>().velocity == Vector3.zero))
            {
                if (player1Turn)
                {
                    player1Ghost.SetActive(true);
                    player1Ghost.transform.position = spawnLocations[column].transform.position;
                } else
                {
                    player2Ghost.SetActive(true);
                    player2Ghost.transform.position = spawnLocations[column].transform.position;
                }
            }
        }

        public void selectColumn(int column)
        {
            if (!gameOver && player1Turn == selectedPlayer1 && (fallingPiece == null || fallingPiece.GetComponent<Rigidbody>().velocity == Vector3.zero))
            {
                Debug.Log("GameManager column: " + column);
                TakeTurn(column);
            }
        }

        void TakeTurn(int column)
        {
            if (updateBoard(column))
            {
                player1Ghost.SetActive(false);
                player2Ghost.SetActive(false);

                fallingPiece = Instantiate(player1Turn ? player1 : player2, spawnLocations[column].transform.position, Quaternion.Euler(new Vector3(90, 0, 0)));
                fallingPiece.transform.parent = PawnContainer.transform;
                fallingPiece.GetComponent<Rigidbody>().velocity = new Vector3(0, 0.1f, 0);
                objects[lastRow, column] = fallingPiece;
                gameOver = winningMove(boardState, player1Turn ? 1 : 2, true);
                if (gameOver)
                {
                    playerWin = player1Turn;

                    Debug.LogWarning("Player " + (player1Turn ? 1 : 2) + " Won!");
                } else if (isDraw(boardState))
                {
                    playerWin = false;
                    gameOver = true;
                    Debug.LogWarning("Game is Draw!");
                }
                player1Turn = !player1Turn;
                if (player1Turn != selectedPlayer1 && !gameOver)
                {
                    StartCoroutine(aiTurn());
                }
            }
        }

        bool updateBoard(int column)
        {
            for (int i = row_count - 1; i >= 0; i--)
            {
                if (boardState[i, column] == 0)
                {
                    boardState[i, column] = player1Turn ? 1 : 2;
                    lastRow = i;
                    Debug.Log("Piece fitted in (" + i + ", " + column + ")");
                    return true;
                }
            }
            Debug.LogWarning("Column " + column + " is full!");
            return false;
        }

        bool winningMove(int[,] board, int player, bool realWin)
        {

            //horizontala
            for (int c = 0; c < column_count - 3; c++)
                for (int r = 0; r < row_count; r++)
                    if (board[r, c] == player && board[r, c + 1] == player && board[r, c + 2] == player && board[r, c + 3] == player)
                    {
                        if (realWin)
                        {
                            StartCoroutine(BlinkGameObject(objects[r, c]));
                            StartCoroutine(BlinkGameObject(objects[r, c + 1]));
                            StartCoroutine(BlinkGameObject(objects[r, c + 2]));
                            StartCoroutine(BlinkGameObject(objects[r, c + 3]));
                        }
                        return true;
                    }

            //vertikala
            for (int c = 0; c < column_count; c++)
                for (int r = 0; r < row_count - 3; r++)
                    if (board[r, c] == player && board[r + 1, c] == player && board[r + 2, c] == player && board[r + 3, c] == player)
                    {
                        if (realWin)
                        {
                            StartCoroutine(BlinkGameObject(objects[r, c]));
                            StartCoroutine(BlinkGameObject(objects[r + 1, c]));
                            StartCoroutine(BlinkGameObject(objects[r + 2, c]));
                            StartCoroutine(BlinkGameObject(objects[r + 3, c]));
                        }
                        return true;
                    }

            //dijagonala y=x
            for (int c = 0; c < column_count - 3; c++)
                for (int r = 0; r < row_count - 3; r++)
                    if (board[r, c] == player && board[r + 1, c + 1] == player && board[r + 2, c + 2] == player && board[r + 3, c + 3] == player)
                    {
                        if (realWin)
                        {
                            StartCoroutine(BlinkGameObject(objects[r, c]));
                            StartCoroutine(BlinkGameObject(objects[r + 1, c + 1]));
                            StartCoroutine(BlinkGameObject(objects[r + 2, c + 2]));
                            StartCoroutine(BlinkGameObject(objects[r + 3, c + 3]));
                        }
                        return true;
                    }

            //dijagonala y=-x
            for (int c = 0; c < column_count - 3; c++)
                for (int r = 3; r < row_count; r++)
                    if (board[r, c] == player && board[r - 1, c + 1] == player && board[r - 2, c + 2] == player && board[r - 3, c + 3] == player)
                    {
                        if (realWin)
                        {
                            StartCoroutine(BlinkGameObject(objects[r, c]));
                            StartCoroutine(BlinkGameObject(objects[r - 1, c + 1]));
                            StartCoroutine(BlinkGameObject(objects[r - 2, c + 2]));
                            StartCoroutine(BlinkGameObject(objects[r - 3, c + 3]));
                        }
                        return true;
                    }

            return false;
        }

        bool isDraw(int[,] board)
        {
            for (int i = 0; i < column_count; i++)
                if (board[0, i] == 0)
                    return false;
            return true;
        }

        List<int> availableColumns(int[,] board)
        {
            List<int> availableColumns = new List<int>();
            for (int i = 0; i < column_count; i++)
                if (board[0, i] == 0)
                    availableColumns.Add(i);
            return availableColumns;
        }

        int getOpenRow(int[,] board, int column)
        {
            for (int i = row_count - 1; i >= 0; i--)
                if (board[i, column] == 0)
                    return i;
            return -1;
        }

        double max(double v1, double v2)
        {
            if (v1 > v2)
                return v1;
            else
                return v2;
        }

        double min(double v1, double v2)
        {
            if (v1 < v2)
                return v1;
            else
                return v2;
        }

        int countScore(List<int> subBoard, int player)
        {
            int score = 0;
            int p1 = 0, p2 = 0, empty = 0;
            for (int i = 0; i < subBoard.Count; i++)
            {
                if (subBoard[i] == 1)
                    p1++;
                else if (subBoard[i] == 2)
                    p2++;
                else
                    empty++;
            }

            if (player == 1)
            {
                if (p1 == 4)
                    score += 100;
                else if (p1 == 3 && empty == 1)
                    score += 5;
                else if (p1 == 2 && empty == 2)
                    score += 2;

                if (p2 == 3 && empty == 1)
                    score -= 4;
            } else
            {
                if (p2 == 4)
                    score += 100;
                else if (p2 == 3 && empty == 1)
                    score += 5;
                else if (p2 == 2 && empty == 2)
                    score += 2;

                if (p1 == 3 && empty == 1)
                    score -= 4;
            }

            return score;
        }

        int scorePosition(int[,] board, int player)
        {
            //Debug.Log("Pozvan score");
            int score = 0;
            for (int i = 0; i < row_count; i++)
                if (board[i, 3] == player)
                    score++;
            score *= 3;
            List<int> subBoard = new List<int>();

            //horiznotala
            for (int r = 0; r < row_count; r++)
                for (int c = 0; c < column_count - 3; c++)
                {
                    for (int i = 0; i < 4; i++)
                        subBoard.Add(board[r, c + i]);
                    score += countScore(subBoard, player);
                    subBoard.Clear();
                }

            //vertiakala
            for (int c = 0; c < column_count; c++)
                for (int r = 0; r < row_count - 3; r++)
                {
                    for (int i = 0; i < 4; i++)
                        subBoard.Add(board[r + i, c]);
                    score += countScore(subBoard, player);
                    subBoard.Clear();
                }

            //dijagonala y=x
            for (int r = 0; r < row_count - 3; r++)
                for (int c = 0; c < column_count - 3; c++)
                {
                    for (int i = 0; i < 4; i++)
                        subBoard.Add(board[r + i, c + i]);
                    score += countScore(subBoard, player);
                    subBoard.Clear();
                }

            //dijagonala y=-x
            for (int r = 0; r < row_count - 3; r++)
                for (int c = 0; c < column_count - 3; c++)
                {
                    for (int i = 0; i < 4; i++)
                        subBoard.Add(board[r + 3 - i, c + i]);
                    score += countScore(subBoard, player);
                    subBoard.Clear();
                }

            return score;
        }

        bool isTerminalNode(int[,] board)
        {
            return winningMove(board, 1, false) || winningMove(board, 2, false) || isDraw(board);
        }

        (int, double) minimax(int[,] board, int depth, double alpha, double beta, bool maximizingPlayer)
        {
            int AIplayer = selectedPlayer1 ? 2 : 1;
            int HumanPlayer = selectedPlayer1 ? 1 : 2;
            List<int> availableCol = availableColumns(board);
            bool terminalNode = isTerminalNode(board);
            //Debug.Log("Terminal Node: " + terminalNode);
            if (depth == 0 || terminalNode)
            {
                if (terminalNode)
                {
                    if (winningMove(board, AIplayer, false))
                        return (-1, 100000000000000);
                    else if (winningMove(board, HumanPlayer, false))
                        return (-1, -100000000000000);
                    else
                        return (-1, 0);
                } else
                {
                    return (-1, scorePosition(board, AIplayer));
                }
            }
            double value;
            int column = availableCol[Random.Range(0, availableCol.Count)];
            if (maximizingPlayer)
            {
                value = double.NegativeInfinity;
                int row;
                int[,] cloneBoard;
                double newScore;
                for (int i = 0; i < availableCol.Count; i++)
                {
                    row = getOpenRow(board, availableCol[i]);
                    //Debug.Log("Check row: " + row + " col: " + availableCol[i]);
                    cloneBoard = (int[,])board.Clone();
                    cloneBoard[row, availableCol[i]] = AIplayer;
                    newScore = minimax(cloneBoard, depth - 1, alpha, beta, false).Item2;
                    //Debug.Log("NewScore: " + newScore);
                    if (newScore > value)
                    {
                        //Debug.Log(depth + "MAX BEST: (" + row + "," + availableCol[i] + ") score: " + newScore);
                        value = newScore;
                        column = availableCol[i];
                    }
                    alpha = max(alpha, value);
                    if (alpha >= beta)
                        break;
                }
                return (column, value);
            } else
            {
                value = double.PositiveInfinity;
                int row;
                int[,] cloneBoard;
                double newScore;
                for (int i = 0; i < availableCol.Count; i++)
                {
                    row = getOpenRow(board, availableCol[i]);
                    //Debug.Log("Check row: " + row + " col: " + availableCol[i]);
                    cloneBoard = (int[,])board.Clone();
                    cloneBoard[row, availableCol[i]] = HumanPlayer;
                    newScore = minimax(cloneBoard, depth - 1, alpha, beta, true).Item2;
                    //Debug.Log("NewScore: " + newScore);
                    if (newScore < value)
                    {
                        //Debug.Log(depth +  "MIN BEST: (" + row + "," + availableCol[i] + ") score: " + newScore);
                        value = newScore;
                        column = availableCol[i];

                    }
                    beta = min(beta, value);
                    if (alpha >= beta)
                        break;
                }
                return (column, value);
            }
        }


        public IEnumerator BlinkGameObject(GameObject gameObject)
        {
            Material winColor, regularColor;
            if (player1Turn)
            {
                regularColor = color1;
                winColor = winColor1;
            } else
            {
                regularColor = color2;
                winColor = winColor2;
            }

            yield return new WaitForSeconds(0.6f);
            for (int i = 0; i < 60; i++)
            {
                gameObject.GetComponent<MeshRenderer>().material = ((i % 2) == 0 ? winColor : regularColor);
                yield return new WaitForSeconds(0.5f);
            }
            gameObject.GetComponent<MeshRenderer>().material = winColor;
            gameObject.SetActive(true);
        }

    }
}