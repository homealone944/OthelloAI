using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class currentBoard : MonoBehaviour
{
    private uiUpdater uiManager;

    //Game Variables
    public Dictionary<Vector2, GameSpot> curBoard;
    [HideInInspector]
    public int whoseTurn = 1;

    //Helper Variables
    private int cantMoveCount = 0;
    bool changed = false;
    [HideInInspector]
    public bool isPlaying = false;
    string[] letters = new string[] {"A", "B", "C", "D", "E", "F", "G", "H"};

    //AI Variables
    public List<AI> aiList;
    bool p1IsAi = false;
    bool p2IsAi = false;

    private void Start()
    {
        uiManager = gameObject.GetComponent<uiUpdater>();
    }

    public void startGame(bool aiP1, bool aiP2)
    {
        whoseTurn = 1;

        aiList[0].gameObject.SetActive(true);
        aiList[1].gameObject.SetActive(true);

        GetComponent<MakeTable>().StartGame();
        p1IsAi = aiP1;
        p2IsAi = aiP2;
        if (!p1IsAi)
            aiList[0].gameObject.SetActive(false);
        if (!p2IsAi)
            aiList[1].gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlaying)
        {
            if (changed)
            {
                changed = false;
                uiManager.setScore(getScores());

                clearPossible();

                //if possible for player to play, show possible moves
                //else go to next player's turn (changes = true)
                List<Vector2> possible = getPossibleMovesForTurn(curBoard);
                if (possible.Count != 0)
                {
                    cantMoveCount = 0;
                    foreach (Vector2 position in possible)
                        curBoard[position].setSpotActive(true);
                }
                else
                {
                    Debug.Log(whoseTurn + ", NO MORE MOVES");
                    cantMoveCount++;
                    if (cantMoveCount < 2)
                    {
                        var player = 2;
                        if (whoseTurn > 0)
                            player = 1;
                        uiManager.addMove(("\tP" + player + ": cant move"));
                        uiManager.setWhoJustMadeTurn(player);

                        goNextTurn();
                    }
                    else
                    {
                        //GameOver
                        Debug.Log("GameOver");
                        changed = false;
                        uiManager.addMove("GAMEOVER: " + getScores());
                    }
                }
            }
        }
        else
        {
            GetComponent<MakeTable>().clearTable();
            uiManager.resetUI();
        }
    }

    public void SpotClicked(Vector2 pos, bool start = false)
    {
        GameSpot cSpot = curBoard[pos];
        var player = 2;
        if (whoseTurn > 0)
            player = 1;

        if (!start)
        {
            uiManager.addMove("\tP" + player + ": (" + letters[(int)pos.x] + "," + pos.y + ")");
            uiManager.setWhoJustMadeTurn(player);
            //Spot is now marked
            cSpot.updateOwn(whoseTurn);

            //FLIPS here
            iterDirection(curBoard, 0, -1, cSpot, true); //Down
            iterDirection(curBoard, 0, 1, cSpot, true); //Up
            iterDirection(curBoard, -1, 0, cSpot, true); //Right
            iterDirection(curBoard, 1, 0, cSpot, true); //Left
            iterDirection(curBoard, 1, 1, cSpot, true); //Up Left
            iterDirection(curBoard, 1, -1, cSpot, true); //Down Left
            iterDirection(curBoard, -1, 1, cSpot, true); //Up Right
            iterDirection(curBoard, -1, -1, cSpot, true); //Down Right
        }

        goNextTurn(start);
    }
    public void clearPossible()
    {
        foreach (Vector2 pos in curBoard.Keys)
        {
            curBoard[pos].setSpotActive(false);
        }
    }
    public void goNextTurn(bool start = false)
    {
        //Switch turn
        changed = true;
        whoseTurn *= -1;
    }

    //Returns a list of spots that can be placed at
    public List<Vector2> getPossibleMovesForTurn(Dictionary<Vector2, GameSpot> currentState)
    {
        /*
        SUDO CODE:
            Narrow down first by only the spots connected to your opponent's tile
            Then loop over: up,down,left,right,diagonals
                if one of following is true: valid spot
        */

        List<Vector2> possible = new List<Vector2>();
        foreach (Vector2 position in currentState.Keys)
        {
            GameSpot s = currentState[position];
            //not already played on this spot and if is next to opponent's tile
            if (s.whoOwns == 0 && isNextToOpponent(currentState, s, whoseTurn * -1))
                possible.Add(position);
        }

        //Narrow down based off direction
        List<Vector2> remove = new List<Vector2>();
        foreach (Vector2 pos in possible)
        {
            GameSpot curSpot = currentState[pos];
            int posX = (int)curSpot.pos.x;
            int posY = (int)curSpot.pos.y;

            if (iterDirection(currentState, 0, -1,curSpot)) //Down
                continue;
            else if (iterDirection(currentState, 0, 1, curSpot)) //Up
                continue;
            else if (iterDirection(currentState , -1, 0, curSpot)) //Right
                continue;
            else if (iterDirection(currentState, 1, 0, curSpot)) //Left
                continue;
            else if (iterDirection(currentState, 1, 1, curSpot)) //Up Left
                continue;
            else if (iterDirection(currentState, 1, -1, curSpot)) //Down Left
                continue;
            else if (iterDirection(currentState, -1, 1, curSpot)) //Up Right
                continue;
            else if (iterDirection(currentState, -1, -1, curSpot)) //Down Right
                continue;
            else
                remove.Add(pos);
        }

        foreach (Vector2 np in remove)
            possible.Remove(np);

        return possible;
    }

    public List<Vector2> getPossibleMovesForTurn(Dictionary<Vector2, Spot> currentState)
    {
        /*
        SUDO CODE:
            Narrow down first by only the spots connected to your opponent's tile
            Then loop over: up,down,left,right,diagonals
                if one of following is true: valid spot
        */

        List<Vector2> possible = new List<Vector2>();
        foreach (Vector2 position in currentState.Keys)
        {
            Spot s = currentState[position];
            //not already played on this spot and if is next to opponent's tile
            if (s.whoOwns == 0 && isNextToOpponent(currentState, s, whoseTurn * -1))
                possible.Add(position);
        }

        //Narrow down based off direction
        List<Vector2> remove = new List<Vector2>();
        foreach (Vector2 pos in possible)
        {
            Spot curSpot = currentState[pos];
            int posX = (int)curSpot.pos.x;
            int posY = (int)curSpot.pos.y;

            if (iterDirection(currentState, 0, -1, curSpot)) //Down
                continue;
            else if (iterDirection(currentState, 0, 1, curSpot)) //Up
                continue;
            else if (iterDirection(currentState, -1, 0, curSpot)) //Right
                continue;
            else if (iterDirection(currentState, 1, 0, curSpot)) //Left
                continue;
            else if (iterDirection(currentState, 1, 1, curSpot)) //Up Left
                continue;
            else if (iterDirection(currentState, 1, -1, curSpot)) //Down Left
                continue;
            else if (iterDirection(currentState, -1, 1, curSpot)) //Up Right
                continue;
            else if (iterDirection(currentState, -1, -1, curSpot)) //Down Right
                continue;
            else
                remove.Add(pos);
        }

        foreach (Vector2 np in remove)
            possible.Remove(np);

        return possible;
    }

    //Will iterate in direction
    //Return: true if startLoc is possible spot for currentPlayer to play
    //Input: change in X + change in Y + position to check if possible spot + if spots are to be flipped
    public bool iterDirection(Dictionary<Vector2, GameSpot> currentState, int deltaX, int deltaY, GameSpot startLoc, bool flip = false, bool print = false)
    {
        List<GameSpot> toFlip = new List<GameSpot>();
        int dirPossible = 0;
        int hitMyOwn = 0;
        int x = (int)startLoc.pos.x + deltaX;
        int y = (int)startLoc.pos.y + deltaY;
        //End if get to edge, first spot != opponent, doesnt ever hit own
        while (inBounds(x, y) && dirPossible != -1 && hitMyOwn != 1)
        {
            GameSpot newSpot = currentState[new Vector2(x, y)];
            if(print) Debug.Log("Checking ["+deltaX+":"+deltaY+"]: " + startLoc.customPrint() + ", " + newSpot.customPrint());
            //First in that direction
            if (dirPossible == 0)
            {
                //First spot to the right of initial
                if(print) Debug.Log("First: " + newSpot.whoOwns);
                if (newSpot.whoOwns == whoseTurn * -1)
                {
                    //Opponent owns it
                    dirPossible = 1;
                    if(print) Debug.Log("Opponent Owns + canFlip");
                    toFlip.Add(newSpot);
                }
                else if (newSpot.whoOwns != whoseTurn * -1)
                {
                    //Oppononent doesnt own it
                    dirPossible = -1;
                    if (print) Debug.Log("Opponent doesnt Owns (endloop)");
                }
            }
            else
            {
                if (newSpot.whoOwns == 0)
                {
                    //hit empty
                    dirPossible = -1;
                    if (print) Debug.Log("Hit empty (end while)");
                }
                if (newSpot.whoOwns == whoseTurn)
                {
                    //Hit my own
                    hitMyOwn = 1;
                    if (print) Debug.Log("Hit own (end while)");
                }
                else  //Opponent
                {
                    if (print) Debug.Log("Flipping");
                    toFlip.Add(newSpot);
                }
            }
            x+=deltaX;
            y+=deltaY;
        }

        bool isPossible = (dirPossible == 1 && hitMyOwn == 1);
        if (print) Debug.Log("Iter found " + startLoc.customPrint() + ": " + isPossible);

        if (!isPossible)
            toFlip.Clear();

        if (flip)
        {
            foreach (GameSpot s in toFlip)
                s.flip();
        }

        return isPossible;
    }

    public bool iterDirection(Dictionary<Vector2, Spot> currentState, int deltaX, int deltaY, Spot startLoc, bool flip = false, bool print = false)
    {
        List<Spot> toFlip = new List<Spot>();
        int dirPossible = 0;
        int hitMyOwn = 0;
        int x = (int)startLoc.pos.x + deltaX;
        int y = (int)startLoc.pos.y + deltaY;
        //End if get to edge, first spot != opponent, doesnt ever hit own
        while (inBounds(x, y) && dirPossible != -1 && hitMyOwn != 1)
        {
            Spot newSpot = currentState[new Vector2(x, y)];
            if (print) Debug.Log("Checking [" + deltaX + ":" + deltaY + "]: " + startLoc.customPrint() + ", " + newSpot.customPrint());
            //First in that direction
            if (dirPossible == 0)
            {
                //First spot to the right of initial
                if (print) Debug.Log("First: " + newSpot.whoOwns);
                if (newSpot.whoOwns == whoseTurn * -1)
                {
                    //Opponent owns it
                    dirPossible = 1;
                    if (print) Debug.Log("Opponent Owns + canFlip");
                    toFlip.Add(newSpot);
                }
                else if (newSpot.whoOwns != whoseTurn * -1)
                {
                    //Oppononent doesnt own it
                    dirPossible = -1;
                    if (print) Debug.Log("Opponent doesnt Owns (endloop)");
                }
            }
            else
            {
                if (newSpot.whoOwns == 0)
                {
                    //hit empty
                    dirPossible = -1;
                    if (print) Debug.Log("Hit empty (end while)");
                }
                if (newSpot.whoOwns == whoseTurn)
                {
                    //Hit my own
                    hitMyOwn = 1;
                    if (print) Debug.Log("Hit own (end while)");
                }
                else  //Opponent
                {
                    if (print) Debug.Log("Flipping");
                    toFlip.Add(newSpot);
                }
            }
            x += deltaX;
            y += deltaY;
        }

        bool isPossible = (dirPossible == 1 && hitMyOwn == 1);
        if (print) Debug.Log("Iter found " + startLoc.customPrint() + ": " + isPossible);

        if (!isPossible)
            toFlip.Clear();

        if (flip)
        {
            foreach (Spot s in toFlip)
                s.flip();
        }

        return isPossible;
    }

    public Vector2 getScores()
    {
        Vector2 scores = new Vector2(0, 0);
        foreach (Vector2 pos in curBoard.Keys)
        {
            GameSpot s = curBoard[pos];
            if (s.whoOwns == 1)
                scores.x = scores.x + 1;
            else if (s.whoOwns == -1)
                scores.y = scores.y + 1;
        }
        return scores;
    }
    public bool isNextToOpponent(Dictionary<Vector2, GameSpot> currentState, GameSpot spot, int opponent)
    {
        List<Vector2> neighbors = spot.getSpotsAround();
        foreach (Vector2 nPos in neighbors)
        {
            GameSpot s = currentState[nPos];
            if (s.whoOwns == opponent)
                return true;
        }
        return false;
    }
    public bool isNextToOpponent(Dictionary<Vector2, Spot> currentState, Spot spot, int opponent)
    {
        List<Vector2> neighbors = spot.getSpotsAround();
        foreach (Vector2 nPos in neighbors)
        {
            Spot s = currentState[nPos];
            if (s.whoOwns == opponent)
                return true;
        }
        return false;
    }
    private bool inBounds(int x, int y)
    {
        return (x >= 0 && x <= 7 && y >= 0 && y <= 7);
    }
}
