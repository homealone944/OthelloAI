using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Spot
{
    public Vector2 pos;

    //-1 = p2, 1 = p1, 0 = none
    public int whoOwns = 0;

    public void updateOwn(int n)
    {
        whoOwns = n;
    }

    public void flip()
    {
        updateOwn(whoOwns * -1);
    }

    public void setSpotActive(bool n)
    {
        updateOwn(whoOwns);
    }

    public List<Vector2> getSpotsAround()
    {
        List<Vector2> allSpots = new List<Vector2>();
        for (int x = (int)pos.x - 1; x <= pos.x + 1; x++)
        {
            for (int y = (int)pos.y - 1; y <= pos.y + 1; y++)
            {
                if (inBounds(x, y))
                    allSpots.Add(new Vector2(x, y));
            }
        }
        //Remove this Spot
        allSpots.Remove(pos);

        return allSpots;
    }

    public string customPrint()
    {
        string locString = "(" + pos.x + "," + pos.y + ")";
        return locString + ": " + whoOwns;
    }

    private bool inBounds(int x, int y)
    {
        return (x >= 0 && x <= 7 && y >= 0 && y <= 7);
    }
}
