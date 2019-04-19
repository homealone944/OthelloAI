using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class uiUpdater : MonoBehaviour
{
    [Header("Scoring")]
    public Text p1ScoreUI;
    private int p1Score = 0;
    public Text p2ScoreUI;
    private int p2Score = 0;
    public Text moveListUI;
    public Image whoseTurnUI;
    public GameObject p1Turn;
    public GameObject p2Turn;

    [Header("AI")]
    public Toggle AiP1;
    public Toggle AiP2;
    public GameObject startButton;
    public GameObject stopButton;

    void Start()
    {
        setScore(new Vector2(p1Score, p2Score));
        startButton.SetActive(true);
        stopButton.SetActive(false);
    }

    public void setScore(Vector2 scores)
    {
        p1Score = (int)scores.x;
        p2Score = (int)scores.y;

        p1ScoreUI.text = "P1: " + p1Score;
        p2ScoreUI.text = "P2: " + p2Score;
    }
    public void addMove(string move)
    {
        moveListUI.text += " " + move + "\n";
    }
    public void setWhoJustMadeTurn(int n)
    {
        Vector2 pos = Vector2.zero;
        if (n == 2)
            pos = p1Turn.transform.position;
        else
            pos = p2Turn.transform.position;

        whoseTurnUI.transform.position = pos;
    }

    public void startStop()
    {
        startButton.SetActive(!startButton.activeInHierarchy);
        stopButton.SetActive(!stopButton.activeInHierarchy);

        currentBoard b = GetComponent<currentBoard>();

        b.isPlaying = !startButton.activeInHierarchy;
        
        if (b.isPlaying)
        {
            addMove("<Start Game>");
            b.startGame(AiP1.isOn, AiP2.isOn);
            AiP1.interactable = false;
            AiP2.interactable = false;
        }
        else
        {
            AiP1.interactable = true;
            AiP2.interactable = true;
        }

    }

    public void resetUI()
    {
        moveListUI.text = "";
        setScore(Vector2.zero);
        setWhoJustMadeTurn(2);
    }
}
