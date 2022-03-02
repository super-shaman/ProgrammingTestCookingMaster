using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

    public Tile KitchenCounterTile;
    public Tile KitchenFloorTile;
    public Tile ServingCounterTile;
    public int width;
    public int height;
    public Player Player1;
    public Player Player2;
    public Vegetable[] Vegetables;
    int[,] types;
    Tile[,] tiles;


    void Start()
    {
        for (int i = 0; i < Vegetables.Length; i++)
        {
            Vegetables[i].type = i;
        }
        types = new int[width, height];
        tiles = new Tile[width, height];
        for (int i = 1; i < width-1; i++)
        {
            types[i, height - 1] = 2;
        }
        for (int i = 1; i < width-1; i++)
        {
            for (int ii = 1; ii < height-1; ii++)
            {
                types[i, ii] = 1;
            }
        }
        for (int i = 0; i < width; i++)
        {
            for (int ii = 0; ii < height; ii++)
            {
                if (types[i,ii] == 0)
                {
                    Tile g = Instantiate(KitchenCounterTile);
                    g.transform.position = new Vector3(-Mathf.FloorToInt(width / 2.0f) + i, -Mathf.FloorToInt(height / 2.0f) + ii, 1);
                    tiles[i, ii] = g;
                }else if (types[i,ii] == 1)
                {
                    Tile g = Instantiate(KitchenFloorTile);
                    g.transform.position = new Vector3(-Mathf.FloorToInt(width / 2.0f) + i, -Mathf.FloorToInt(height / 2.0f) + ii, 1);
                    tiles[i, ii] = g;
                }
                else if (types[i,ii] == 2)
                {
                    Tile g = Instantiate(ServingCounterTile);
                    g.transform.position = new Vector3(-Mathf.FloorToInt(width / 2.0f) + i, -Mathf.FloorToInt(height / 2.0f) + ii, 1);
                    tiles[i, ii] = g;
                }
            }
        }
        for (int i = 0; i < Vegetables.Length/2; i++)
        {
            Entity e = Instantiate(Vegetables[i]);
            Tile t = tiles[0, 3 + i];
            t.occupant = e;
            e.transform.position = t.transform.position;
        }
        for (int i = 0; i < Vegetables.Length / 2; i++)
        {
            Entity e = Instantiate(Vegetables[i+Vegetables.Length/2]);
            Tile t = tiles[width - 1, 3 + i];
            t.occupant = e;
            e.transform.position = t.transform.position;
        }
        Player1.index = new Vector2Int(1, Mathf.FloorToInt(height / 2.0f));
        Player2.index = new Vector2Int(width-2, Mathf.FloorToInt(height / 2.0f));
        Player1.target = tiles[Player1.index.x, Player1.index.y];
        Player2.target = tiles[Player2.index.x, Player2.index.y];
        Player1.target.occupant = Player1;
        Player2.target.occupant = Player2;
    }

    void Move(Player p, Vector2Int dir)
    {
        Vector2Int v = p.index;
        v += dir;
        Entity e = tiles[v.x, v.y].occupant;
        if (e != null)
        {
            if (e.ClassType == 1)
            {
                Vegetable veg = e.GetComponent<Vegetable>();
                p.AddEntity(Instantiate(Vegetables[veg.type]));
            }else if (e.ClassType == 0)
            {
                return;
            }
        }
        v.x = v.x == 0 ? 1 : v.x == width - 1 ? width - 2 : v.x;
        v.y = v.y == 0 ? 1 : v.y == height - 1 ? height - 2 : v.y;
        p.index = v;
        p.target.occupant = null;
        p.target = tiles[p.index.x, p.index.y];
        p.target.occupant = p;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2Int p1 = new Vector2Int();
        if (Input.GetKeyDown(KeyCode.W))
        {
            p1.y++;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            p1.x--;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            p1.y--;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            p1.x++;
        }
        if (p1.magnitude > 0)
        {
            Move(Player1, p1);
        }
        Vector2Int p2 = new Vector2Int();
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            p2.y++;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            p2.x--;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            p2.y--;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            p2.x++;
        }
        if (p2.magnitude > 0)
        {
            Move(Player2, p2);
        }
    }
}
