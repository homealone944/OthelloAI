﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AI : MonoBehaviour
{
    public int playerNum = 0;
    private int maxDepth = 10;
    private currentBoard cb;
    private List<Vector2> possibleMoves;

    void Start()
    {
        cb = GameObject.FindGameObjectWithTag("GameController").GetComponent<currentBoard>();
    }

    void Update()
    {
        if (cb.whoseTurn == playerNum && cb.isPlaying)
        {
            Vector2 spot = onTurn(cb.curBoard, cb);
            if (spot != new Vector2(-1, -1))
                cb.SpotClicked(spot);
        }
    }

    //What AI will do when the currentBoard asks for AI's move
    public Vector2 onTurn(Dictionary<Vector2, Spot> currentState, currentBoard cb)
    {
        Dictionary<Vector2, Spot> copyState = copyDict(currentState);

        Vector2 decision;
        possibleMoves = cb.getPossibleMovesForTurn(currentState);
        if (possibleMoves.Count == 0)
        {
            return new Vector2(-1, -1);
        }

        //TODO Make minMax tree with AlphaBetaPruning here
        var nextSpot = maxVal(copyState, double.MinValue, double.MaxValue, 0);

        //decision = nextSpot.Item2;
        decision = currentState[nextSpot.Item2].pos;

        //Below is placeholder
        //decision = possibleMoves[Random.Range(0,possibleMoves.Count)];

        return decision;
    }

    private Tuple<double, Vector2> maxVal(Dictionary<Vector2, Spot> state, double alpha, double beta, int depth)
    {
        List<Vector2> myPossibleMoves = cb.getPossibleMovesForTurn(state);

        if (depth >= maxDepth)
        {
            return Tuple.Create(getUtilityMax(getScores(state)), new Vector2(-1,-1));
        }
        Dictionary<Vector2, Spot>[] nextStates = new Dictionary<Vector2, Spot>[myPossibleMoves.Count];
        var utilityVal = Tuple.Create(double.MinValue, new Vector2(-1,-1));
        for (int i = 0; i < nextStates.Length; i++)
        {
            nextStates[i] = copyDict(state);
            //nextStates[i] = state; // Should create another deep copy here
            SpotClicked(nextStates[i], myPossibleMoves[i], playerNum);
            var v = minVal(nextStates[i], alpha, beta, depth + 1);
            if(utilityVal.Item1 < v.Item1)
            {
                utilityVal = Tuple.Create(v.Item1, myPossibleMoves[i]);
            }
            if (utilityVal.Item1 >= beta)
            {
                return utilityVal;
            }
            alpha = Math.Max(alpha, utilityVal.Item1);
        }
        return utilityVal;
    }

    private Tuple<double, Vector2> minVal(Dictionary<Vector2, Spot> state, double alpha, double beta, int depth)
    {
        List<Vector2> myPossibleMoves = cb.getPossibleMovesForTurn(state);

        if (depth >= maxDepth)
        {
            return Tuple.Create(getUtilityMin(getScores(state)), new Vector2(-1, -1));
        }
        Dictionary<Vector2, Spot>[] nextStates = new Dictionary<Vector2, Spot>[myPossibleMoves.Count];
        var utilityVal = Tuple.Create(double.MaxValue, new Vector2(-1, -1));
        for (int i = 0; i < nextStates.Length; i++)
        {
            nextStates[i] = copyDict(state);
            //nextStates[i] = state;
            SpotClicked(nextStates[i], myPossibleMoves[i], playerNum);
            var v = maxVal(nextStates[i], alpha, beta, depth + 1);
            if (utilityVal.Item1 > v.Item1)
            {
                utilityVal = Tuple.Create(v.Item1, myPossibleMoves[i]);
            }
            if (utilityVal.Item1 <= alpha)
            {
                return utilityVal;
            }
            beta = Math.Min(beta, utilityVal.Item1);
        }
        return utilityVal;
    }

    private Dictionary<Vector2, Spot> copyDict(Dictionary<Vector2, Spot> state)
    {
        Dictionary<Vector2, Spot> copyState = new Dictionary<Vector2, Spot>();
        foreach (KeyValuePair<Vector2, Spot> item in state)
        {
            Spot spot = new Spot(); // This doesn't work, unity won't let you create a new Spot
            spot.pos = new Vector2(item.Key.x, item.Key.y);
            copyState.Add(new Vector2(item.Key.x, item.Key.y), spot);
        }
        return copyState;
    }

    private Vector2 getScores(Dictionary<Vector2, Spot> board)
    {
        Vector2 scores = new Vector2(0, 0);
        foreach (Vector2 pos in board.Keys)
        {
            Spot s = board[pos];
            if (s.whoOwns == 1)
                scores.x = scores.x + 1;
            else if (s.whoOwns == -1)
                scores.y = scores.y + 1;
        }
        return scores;
    }

    private double getUtilityMax(Vector2 scores)
    {
        return scores.x - scores.y;
    }

    private double getUtilityMin(Vector2 scores)
    {
        return scores.y - scores.x;
    }

    private void SpotClicked(Dictionary<Vector2, Spot> board, Vector2 pos, int whoClicked)
    {
        Spot cSpot = board[pos];

        //Spot is now marked
        cSpot.updateOwn(whoClicked);

        //FLIPS here
        cb.iterDirection(board, 0, -1, cSpot, true); //Down
        cb.iterDirection(board, 0, 1, cSpot, true); //Up
        cb.iterDirection(board, -1, 0, cSpot, true); //Right
        cb.iterDirection(board, 1, 0, cSpot, true); //Left
        cb.iterDirection(board, 1, 1, cSpot, true); //Up Left
        cb.iterDirection(board, 1, -1, cSpot, true); //Down Left
        cb.iterDirection(board, -1, 1, cSpot, true); //Up Right
        cb.iterDirection(board, -1, -1, cSpot, true); //Down Right
    }
}
