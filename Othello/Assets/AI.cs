using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public int playerNum = 0;
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
            Vector2 spot = onTurn(cb.curBoard, cb, cb.whoseTurn);
            if (spot != new Vector2(-1, -1))
                cb.SpotClicked(spot);
        }
    }

    //What AI will do when the currentBoard asks for AI's move
    public Vector2 onTurn(Dictionary<Vector2, Spot> currentState, currentBoard cb, int whoseTurn)
    {
        Vector2 decision;
        possibleMoves = cb.getPossibleMovesForTurn(currentState);
        if (possibleMoves.Count == 0)
            return new Vector2(-1,-1);

        //TODO Make minMax tree with AlphaBetaPruning here

        //Below is placeholder
        decision = possibleMoves[Random.Range(0,possibleMoves.Count)];


        return decision;
    }
    
}
