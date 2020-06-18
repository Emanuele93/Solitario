using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Suit { Hearts, Tiles, Clovers, Pikes }

public class GameManager : MonoBehaviour
{
    private List<UndoMove> undoActions = new List<UndoMove>();

    public Deck deck;
    public TableauStack[] tableauStack;
    public FoundationStack[] foundationStacks;
    public Text pointsText, movesText;
    public Button undoButton, restartButton1, restartButton2, continueButton, pauseButton, homeButton;
    public GameObject board;

    public GameObject victoryPanel, pausePanel;
    public Text victoryPointsText, victoryMovesText, victoryTimeText;
    private float startTime;

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    void Start()
    {
        startTime = Time.time;

        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        CreateCards();

        Button btn = undoButton.GetComponent<Button>();
        btn.onClick.AddListener(UndoLastMove);
        btn = pauseButton.GetComponent<Button>();
        btn.onClick.AddListener(Pause);
        btn = continueButton.GetComponent<Button>();
        btn.onClick.AddListener(Continue);
        btn = restartButton1.GetComponent<Button>();
        btn.onClick.AddListener(Restart);
        btn = restartButton2.GetComponent<Button>();
        btn.onClick.AddListener(Restart);
        btn = homeButton.GetComponent<Button>();
        btn.onClick.AddListener(GoHome);
    }

    private void CreateCards()
    {
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 13; j++)
                deck.CreateCard(i, j + 1);

        deck.Shuffle();

        StartCoroutine(SetBoard());
    }

    private IEnumerator SetBoard()
    {
        for (int i = 0; i < tableauStack.Count(); i++)
        {
            for (int j = 0; j < i + 1; j++)
            {
                tableauStack[i].AddCard(deck.GetCard(), j == i);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public bool GetHelp(Card card)
    {
        foreach (Stack s in foundationStacks)
            if (s.HeplConnectCard(card))
                return true;
        foreach (Stack s in tableauStack)
            if (s.HeplConnectCard(card))
                return true;
        return false;
    }

    public void AddUndoMove(UndoMove action)
    {
        undoActions.Add(action);
        movesText.text = (int.Parse(movesText.text) + 1).ToString();
        int OldPoints = int.Parse(pointsText.text);
        pointsText.text = Mathf.Max(int.Parse(pointsText.text) + action.points, 0).ToString();
        action.points = OldPoints;

    }

    public void EndGame()
    {
        bool end = true;
        foreach (FoundationStack s in foundationStacks)
            end = end && s.Completed();

        if (end)
        {
            board.GetComponent<BoxCollider2D>().enabled = true;
            float timer = Time.time - startTime;
            victoryTimeText.text = string.Format("{0}:{1:00}", (int)timer / 60, (int)timer % 60);
            victoryPointsText.text = pointsText.text;
            victoryMovesText.text = movesText.text;
            victoryPanel.SetActive(true);
        }
    }

    public void UndoLastMove()
    {
        if (undoActions.Count == 0)
            return;
        int points = undoActions[undoActions.Count - 1].Undo();
        movesText.text = (int.Parse(movesText.text) + 1).ToString();
        pointsText.text = (points).ToString();
        undoActions.RemoveAt(undoActions.Count - 1);
    }

    public void Restart()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void Continue()
    {
        board.GetComponent<BoxCollider2D>().enabled = false;
        pausePanel.SetActive(false);
        startTime = Time.time - startTime;
    }

    public void Pause()
    {
        board.GetComponent<BoxCollider2D>().enabled = true;
        pausePanel.SetActive(true);
        startTime = Time.time - startTime;
    }

    public void GoHome()
    {
        SceneManager.LoadScene("HomeScene");
    }
}