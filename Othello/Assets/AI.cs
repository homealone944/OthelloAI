using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    private List<Vector2> possibleMoves;

    //What AI will do when the currentBoard asks for AI's move
    public Vector2 onTurn(Dictionary<Vector2, Spot> currentState, currentBoard cb, int whoseTurn)
    {
        Vector2 decision;
        possibleMoves = cb.getPossibleMovesForTurn(currentState);
        if (possibleMoves.Count == 0)
            return new Vector2(0,0);
        //TODO Make minMax tree with AlphaBetaPruning here

        //Below is placeholder
        decision = possibleMoves[Random.Range(0,possibleMoves.Count)];


        return decision;
    }
    
}
