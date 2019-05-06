using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameSpot : MonoBehaviour
{
    public Button btn;
    public Spot spot;
    /*public Vector2 pos
    {
        get
        {
            return spot.pos;
        }
    }

    //-1 = p2, 1 = p1, 0 = none
    public int whoOwns
    {
        get
        {
            Debug.Log("request who own" + pos);
            return spot.whoOwns;
        }
    }*/

    

    private GameObject manager;

    public Color white;
    public Color black;
    public Color available;
    public Color unavailable;

    public List<Vector2> start;

    void Start()
    {
        btn = this.gameObject.GetComponent<Button>();
        manager = GameObject.FindGameObjectWithTag("GameController");

        int xPos, yPos;
        int.TryParse(btn.name.Substring(0, 1), out xPos);
        int.TryParse(btn.name.Substring(2, 1), out yPos);

        spot = new Spot();
        spot.pos = new Vector2(xPos, yPos);


        if (start.Contains(spot.pos))
        {
            if (spot.pos.x == spot.pos.y)
                updateOwn(-1);
            else
                updateOwn(1);
            pressed(true);
        }
        else
            updateOwn(0);

    }

    public void pressed(bool start)
    {
        setSpotActive(false);
        manager.GetComponent<currentBoard>().SpotClicked(spot.pos, start);
    }

    public void updateOwn(int n)
    {
        spot.updateOwn(n);

        //set color
        if (n == 1)
        {
            setColor(white);
            btn.interactable = false;
            return;
        }
        else if (n == -1)
        {
            setColor(black);
            btn.interactable = false;
            return;
        }
        if (btn.IsInteractable())
            setColor(available);
        else
            setColor(unavailable);
    }
    public void flip()
    {
        spot.flip();
    }
    public void setSpotActive(bool n)
    {
        btn.interactable = n;
        spot.setSpotActive(n);
        updateOwn(spot.whoOwns);
    }
    public List<Vector2> getSpotsAround()
    {
        List<Vector2> allSpots = new List<Vector2>();
        for (int x = (int)spot.pos.x - 1; x <= spot.pos.x + 1; x++)
        {
            for (int y = (int)spot.pos.y - 1; y <= spot.pos.y + 1; y++)
            {
                if (inBounds(x, y))
                    allSpots.Add(new Vector2(x, y));
            }
        }
        //Remove this Spot
        allSpots.Remove(spot.pos);

        return allSpots;
    }
    void setColor(Color c)
    {
        GetComponent<Image>().color = c;
    }

    //This function returns a list of positions that are horizontal,verical, or diagonal to the current Spot
    public List<Vector2> getPositionsToFlip()
    {
        List<Vector2> possible = new List<Vector2>();

        //Horizontal
        for (int xDis = 0; xDis <= 7; xDis++)
        {
            Vector2[] horizontals = new Vector2[2];
            horizontals[0] = new Vector2(spot.pos.x + xDis, spot.pos.y);
            horizontals[1] = new Vector2(spot.pos.x - xDis, spot.pos.y);

            foreach (Vector2 e in horizontals)
            {
                if (e != spot.pos && (e.x >= 0 && e.x <= 7) && (e.y >= 0 && e.y <= 7))
                    possible.Add(e);
            }
        }
        //Vertical
        for (int yDis = 0; yDis <= 7; yDis++)
        {
            Vector2[] verticals = new Vector2[2];
            verticals[0] = new Vector2(spot.pos.x, spot.pos.y + yDis);
            verticals[1] = new Vector2(spot.pos.x, spot.pos.y - yDis);

            foreach (Vector2 e in verticals)
            {
                if (e != spot.pos && (e.x >= 0 && e.x <= 7) && (e.y >= 0 && e.y <= 7))
                    possible.Add(e);
            }
        }
        //Diagonal
        for (int dia = 1; dia < 7; dia++)
        {
            Vector2[] diagonals = new Vector2[4];
            diagonals[0] = new Vector2(spot.pos.x + dia, spot.pos.y + dia);
            diagonals[1] = new Vector2(spot.pos.x - dia, spot.pos.y - dia);
            diagonals[2] = new Vector2(spot.pos.x + dia, spot.pos.y - dia);
            diagonals[3] = new Vector2(spot.pos.x - dia, spot.pos.y + dia);

            foreach (Vector2 e in diagonals)
            {
                if (e != spot.pos && (e.x >= 0 && e.x <= 7) && (e.y >= 0 && e.y <= 7))
                    possible.Add(e);
            }
        }
        return possible;
    }

    public string customPrint()
    {
        string locString = "(" + spot.pos.x + "," + spot.pos.y + ")";
        return locString + ": " + spot.whoOwns;
    }

    private bool inBounds(int x, int y)
    {
        return (x >= 0 && x <= 7 && y >= 0 && y <= 7);
    }
}
