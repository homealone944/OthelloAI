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
    public Text uiTimer;

    public Text outcomes;

    [Header("AI")]
    public Toggle AiP1;
    public Toggle AiP2;
    public GameObject startButton;
    public GameObject stopButton;
    public Dropdown AiP1H;
    public Dropdown AiP2H;
    public Slider AiP1DepthSlider;
    public Slider AiP2DepthSlider;

    private bool timerOn = false;
    private float timer;
    private int gameCount;

    void Start()
    {
        gameCount = 0;
        setScore(new Vector2(p1Score, p2Score));
        startButton.SetActive(true);
        stopButton.SetActive(false);
        AiP1H.value = 0;
        AiP2H.value = 0;
        timer = 0;
    }

    private void Update()
    {
        if(timerOn)
            timer += Time.deltaTime;
        uiTimer.text = "Timer: " + Mathf.Floor(timer) + "s";
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
        resetUI();
        startButton.SetActive(!startButton.activeInHierarchy);
        stopButton.SetActive(!stopButton.activeInHierarchy);

        AiP1DepthSlider.interactable = !AiP1DepthSlider.interactable;
        AiP2DepthSlider.interactable = !AiP2DepthSlider.interactable;

        currentBoard b = GetComponent<currentBoard>();

        b.isPlaying = !startButton.activeInHierarchy;
        
        if (b.isPlaying)
        {
            timerOn = true;
            addMove("<Start Game>");
            b.startGame(AiP1.isOn, AiP2.isOn, AiP1H.value, AiP2H.value, (int)AiP1DepthSlider.value, (int)AiP2DepthSlider.value);
            AiP1.interactable = false;
            AiP2.interactable = false;

        }
        else
        {
            timerOn = false;
            timer = 0;
            AiP1.interactable = true;
            AiP2.interactable = true;
        }

    }

    public void addResult(Vector2 scores)
    {
        gameCount++;
        string add = gameCount+") ";
        if (AiP1.isOn)
        {
            add += "Ai1 " + AiP1H.options[AiP1H.value].text + " D" + AiP1DepthSlider.value +": ";
        }
        else
            add += "P1:";
        add += scores.x + "\n";

        if (AiP2.isOn)
        {
            add += "Ai2 " + AiP2H.options[AiP2H.value].text + " D" + AiP2DepthSlider.value + ": ";
        }
        else
            add += "P2:";
        add += scores.y + "\n";

        add += timer.ToString("0.000") + "s\n";
        outcomes.text += add;
        timerOn = false;
        timer = 0;
    }

    public void resetUI()
    {
        moveListUI.text = "";
        setScore(Vector2.zero);
        setWhoJustMadeTurn(2);
    }

    public void updateDepthShow(Text ui)
    {
        if (ui.name.Contains("1"))
        {
            ui.text = AiP1DepthSlider.value + "";
        }
        else if (ui.name.Contains("2"))
        {
            ui.text = AiP2DepthSlider.value + "";
        }
    }

    public void clearResults()
    {
        outcomes.text = "";
        gameCount = 0;
    }
}
