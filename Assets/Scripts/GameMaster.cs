using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = System.Random;

public class GameMaster : MonoBehaviour
{
    #region variables
    public int selectedRow;
    public int selectedCol;
    public int playerOneScore;
    public int playerTwoScore;

    Random random = new Random();
    private bool playerTurn = false;
    private bool isAIHandled = false;
    private bool isAI = true;
    private bool isTie = false;
    private bool isWinner = false;
    private int winnerNumber;

    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject tieText;
    [SerializeField] private GameObject lineRenderer;
    [SerializeField] private GameObject resetButton;
    [SerializeField] private GameObject[,] buttonTextsArray;
    [SerializeField] private GameObject[] buttonTexts;
    [SerializeField] private GameObject[] winLines;
    [SerializeField] private Text[] scores;
    [SerializeField] private Text[] players;
    [SerializeField] private AudioSource moveSound;
    [SerializeField] private AudioSource winSound;


    //sets all values in the board to 0, unpopulated
    public int[,] board = new int[3, 3]
    {
        {0, 0, 0},
        {0, 0, 0},
        {0, 0, 0},
    };

    #endregion

    private void Start()
    {
        changePlayer();
        buttonTextsArray = new GameObject[3, 3];
        fillButtonTextsArray();
    }
    private void Update()
    {
        if ((!isWinner) && (!playerTurn) && (isAI) && !isAIHandled)
        {
            StartCoroutine(placeAI());
            
        }
    }

    void fillButtonTextsArray()
    {
        int count = 0;

        for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                buttonTextsArray[i, j] = buttonTexts[count];
                count++;
            }
        }
    }

    //methods to place down player's piece
    #region Place

    //sets the row of the selected button
    public void getPositionRow(int row)
    {
         selectedRow = row;
    }

    //sets the column of the selected button
    public void getPositionCol(int col)
    {
         selectedCol = col;
    }

    //sets the text of the selected button if it is empty and it is the player's turn
    public void setPositionText(GameObject button)
    {
        if ((!isWinner) && playerTurn && board[selectedRow, selectedCol] == 0)
        {
            button.GetComponentInChildren<Text>().text = "X";
            board[selectedRow, selectedCol] = 1;
            moveSound.Play();
            checkForWinner();
            changePlayer();
        }
        //two people playing
        else if ((!isWinner) && (!playerTurn) && (board[selectedRow, selectedCol] == 0) && (!isAI))
        {
            button.GetComponentInChildren<Text>().text = "O";
            board[selectedRow, selectedCol] = 2;
            moveSound.Play();
            checkForWinner();
            changePlayer();
        }
    }

    void changePlayer()
    {
        if(!isWinner)
        {
            playerTurn = !playerTurn;
            if(playerTurn)
            {
                players[0].color = Color.green;
                players[1].color = Color.red;
            }
            else
            {
                players[1].color = Color.green;
                players[0].color = Color.red;
            }
        }

    }


    private IEnumerator placeAI()
    {
        isAIHandled = true;

        yield return new WaitForSeconds(1.0f);
        setAIPosition();
        moveSound.Play();
        checkForWinner();
        changePlayer();

        isAIHandled = false;
    }
    #endregion

    //methods to handle the logic of the game
    #region Logic

    public void reset()
    {
        resetButton.SetActive(false);
        winText.SetActive(false);
        tieText.SetActive(false);

        foreach(GameObject text in buttonTexts)
        {
            text.GetComponent<Text>().text = "";
        }

        foreach(GameObject winLine in winLines)
        {
            winLine.SetActive(false);
        }

        board = new int[3, 3]
        {
            {0, 0, 0},
            {0, 0, 0},
            {0, 0, 0},
        };

        selectedRow = 0;
        selectedCol = 0;
        isWinner = false;
        isTie = false;
        //changePlayer();
    }

    //searches for the correct win condition board, if it exists the player wins
    void checkForWinner()
    {
        //top row
        if ((board[0, 0] != 0) && (board[0, 0] == board[0, 1]) && (board[0, 0] == board[0, 2]))
        {
            winnerNumber = (board[0, 0] == 1) ? 1 : 2;
            isWinner = true;
            winLines[0].SetActive(true);
        }

        //middle row
        if ((board[1, 0] != 0) && (board[1, 0] == board[1, 1]) && (board[1, 0] == board[1, 2]))
        {
            winnerNumber = (board[1, 0] == 1) ? 1 : 2;
            isWinner = true;
            winLines[1].SetActive(true);
        }

        //bottom row
        if ((board[2, 0] != 0) && (board[2, 0] == board[2, 1]) && (board[2, 0] == board[2, 2]))
        {
            winnerNumber = (board[2, 0] == 1) ? 1 : 2;
            isWinner = true;
            winLines[2].SetActive(true);
        }

        //left column
        if ((board[0, 0] != 0) && (board[0, 0] == board[1, 0]) && (board[0, 0] == board[2, 0]))
        {
            winnerNumber = (board[0, 0] == 1) ? 1 : 2;
            isWinner = true;
            winLines[3].SetActive(true);
        }

        //middle column
        if ((board[0, 1] != 0) && (board[0, 1] == board[1, 1]) && (board[0, 1] == board[2, 1]))
        {
            winnerNumber = (board[0, 1] == 1) ? 1 : 2;
            isWinner = true;
            winLines[4].SetActive(true);
        }

        //right column
        if ((board[0, 2] != 0) && (board[0, 2] == board[1, 2]) && (board[0, 2] == board[2, 2]))
        {
            winnerNumber = (board[0, 2] == 1) ? 1 : 2;
            isWinner = true;
            winLines[5].SetActive(true);
        }

        //positive diagonal
        if ((board[0, 2] != 0) && (board[0, 2] == board[1, 1]) && (board[0, 2] == board[2, 0]))
        {
            winnerNumber = (board[0, 2] == 1) ? 1 : 2;
            isWinner = true;
            winLines[6].SetActive(true);
        }

        //negative diagonal
        if ((board[0, 0] != 0) && (board[0, 0] == board[1, 1]) && (board[0, 0] == board[2, 2]))
        {
            winnerNumber = (board[0, 0] == 1) ? 1 : 2;
            isWinner = true;
            winLines[7].SetActive(true);
        }

        //board filled/tie
        if(!isWinner && (board[0, 0] != 0) && (board[0, 1] != 0) && (board[0, 2] != 0) && (board[1, 0] != 0) && (board[1, 1] != 0) && (board[1, 2] != 0) && (board[2, 0] != 0) && (board[2, 1] != 0) && (board[2, 2] != 0))
        {
            winnerNumber = 0;
            isTie = true;
        }

        if(isWinner || isTie)
        {
            if (winnerNumber == 0)
            {
                tieText.SetActive(true);
                resetButton.SetActive(true);
                return;
            }
            else if (winnerNumber == 1)
            {
                playerOneScore++;
                scores[0].text = playerOneScore.ToString();
                playerTurn = true;
                winText.SetActive(true);
            }
            else if (winnerNumber == 2)
            {
                playerTwoScore++;
                scores[1].text = playerTwoScore.ToString();
                playerTurn = false;
                winText.SetActive(true);
            }

            winSound.Play();
            resetButton.SetActive(true);
        }
    }
    #endregion

    //methods to manage the ai
    #region AI
    public void setAIPosition()
    {
        selectedRow = new Random().Next(0, 3);
        selectedCol = new Random().Next(0, 3);

        if (board[selectedRow, selectedCol] == 0)
        {
            buttonTextsArray[selectedRow, selectedCol].GetComponent<Text>().text = "O";
            board[selectedRow, selectedCol] = 2;
        }
        else
        {
            setAIPosition();
        }
    }


    #endregion
}
