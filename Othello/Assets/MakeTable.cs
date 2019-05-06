using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MakeTable : MonoBehaviour
{
    public Vector2 topLeft;
    public float addition;

    public float size;

    public GameObject tilePrefab;
    public GameObject tileParent;


    public Dictionary<Vector2, GameSpot> board;

    public void StartGame()
    {
        board = new Dictionary<Vector2, GameSpot>();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                createButton(i, j);
            }
        }
        GameObject manager = GameObject.FindGameObjectWithTag("GameController");
        manager.GetComponent<currentBoard>().curBoard = board;
    }

    public void clearTable()
    {
        foreach (Transform child in tileParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

    }

    void createButton(int x, int y)
    {
        Vector2 loc = new Vector2((x*20)+topLeft.x,topLeft.y - (y*20));
        GameObject tile = Instantiate(tilePrefab, loc, Quaternion.identity, tileParent.transform);
        tile.name = x + "," + y;

        board.Add(new Vector2(x,y),tile.GetComponent<GameSpot>());
    }
}
