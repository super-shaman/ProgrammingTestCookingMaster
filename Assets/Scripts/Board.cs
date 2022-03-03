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
    public Canvas canvas;
    public Customer customer;
    public GameObject plusSign;
    public ChoppingBoard choppingBoard;
    public PlateRack plateRack;
    int[,] types;
    Tile[,] tiles;
    public static Board board;

    Vector3 offset = new Vector3(0, 0, -1.0f / 16.0f);

    void Start()
    {
        board = this;
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
            e.transform.position = t.transform.position+ offset;
        }
        for (int i = 0; i < Vegetables.Length / 2; i++)
        {
            Entity e = Instantiate(Vegetables[i+Vegetables.Length/2]);
            Tile t = tiles[width - 1, 3 + i];
            t.occupant = e;
            e.transform.position = t.transform.position+ offset;
        }
        Player1.index = new Vector2Int(1, Mathf.FloorToInt(height / 2.0f));
        Player2.index = new Vector2Int(width-2, Mathf.FloorToInt(height / 2.0f));
        Player1.target = tiles[Player1.index.x, Player1.index.y];
        Player2.target = tiles[Player2.index.x, Player2.index.y];
        Player1.target.occupant = Player1;
        Player2.target.occupant = Player2;
        for (int i = 1; i < width-1; i++)
        {
            SpawnCustomerOnTile(tiles[i, height - 1]);
        }
        SpawnChoppingBoard(Mathf.FloorToInt(width / 2.0f) - 1, 0, Player1);
        SpawnChoppingBoard(Mathf.FloorToInt(width / 2.0f) + 1, 0, Player2);
        PlateRack pr = Instantiate(plateRack);
        pr.transform.position = tiles[Mathf.FloorToInt(width / 2.0f), 0].transform.position+ offset;

    }

    void SpawnChoppingBoard(int i, int ii, Player p)
    {
        ChoppingBoard cb = Instantiate(choppingBoard);
        Tile choppingBoardTile = tiles[i,ii];
        cb.transform.position = choppingBoardTile.transform.position+ offset;
        choppingBoardTile.occupant = cb;
        cb.player = p;
        cb.transform.SetParent(canvas.transform, false);
    }

    public IEnumerator SpawnCustomer(Tile tile)
    {
        yield return new WaitForSeconds(Random.Range(0, 5.0f));
        Customer c = Instantiate(customer);
        c.transform.SetParent(canvas.transform, false);
        c.target = tile;
        c.transform.position = c.target.transform.position + new Vector3(0, 5, 0);
        c.Load(Random.Range(1.0f, 3.0f));
        c.target.occupant = c;
    }

    public void SpawnCustomerOnTile(Tile tile)
    {
        StartCoroutine(SpawnCustomer(tile));
    }

    void Check(Player p, Vector2Int v)
    {
        Entity e = tiles[v.x, v.y].occupant;
        if (e != null)
        {
            if (e.ClassType == 1)
            {
                Vegetable veg = e.GetComponent<Vegetable>();
                if (p.GetHoldCount() < 2)
                {
                    p.AddEntity(Instantiate(Vegetables[veg.type]));
                }
            }else if (e.ClassType == 2)
            {
                Plate plate = p.GetPlate();
                if (plate != null)
                {
                    Customer c = e.GetComponent<Customer>();
                    if (c.TakeOrder(plate))
                    {
                        p.time += 10;
                        p.points += 100;
                        p.SetPoints();
                    }else
                    {
                        p.time -= 5;
                        p.points -= 50;
                        p.SetPoints();
                    }
                }
            }else if (e.ClassType == 3)
            {
                ChoppingBoard cb = e.GetComponent<ChoppingBoard>();
                if (cb.CheckPlayer(p))
                {
                    Vegetable veg = p.GetVegetable();
                    if (veg != null)
                    {
                        cb.AddVegetable(veg);
                    }else
                    {
                        cb.Chop();
                    }
                }

            }
            else if (e.ClassType == 0)
            {
                return;
            }
        }
    }

    void Move(Player p, Vector2Int pos)
    {
        p.index = pos;
        p.target.occupant = null;
        p.target = tiles[p.index.x, p.index.y];
        p.target.occupant = p;
    }

    void UpdatePlayer(Player player, KeyCode up, KeyCode left, KeyCode down , KeyCode right)
    {
        if (Input.GetKeyDown(up))
        {
            Vector2Int v = player.index;
            v.y++;
            Check(player, v);
            if (v.x >= 1 && v.x < width - 1 && v.y >= 1 && v.y < height - 1)
            {
                Move(player, v);
            }
        }
        if (Input.GetKeyDown(left))
        {
            Vector2Int v = player.index;
            v.x--;
            Check(player, v);
            if (v.x >= 1 && v.x < width - 1 && v.y >= 1 && v.y < height - 1)
            {
                Move(player, v);
            }
        }
        if (Input.GetKeyDown(down))
        {
            Vector2Int v = player.index;
            v.y--;
            Check(player, v);
            if (v.x >= 1 && v.x < width - 1 && v.y >= 1 && v.y < height - 1)
            {
                Move(player, v);
            }
        }
        if (Input.GetKeyDown(right))
        {
            Vector2Int v = player.index;
            v.x++;
            Check(player, v);
            if (v.x >= 1 && v.x < width - 1 && v.y >= 1 && v.y < height - 1)
            {
                Move(player, v);
            }
        }
        Vector2Int p1 = new Vector2Int();
        if (Input.GetKey(up))
        {
            p1.y++;
        }
        if (Input.GetKey(left))
        {
            p1.x--;
        }
        if (Input.GetKey(down))
        {
            p1.y--;
        }
        if (Input.GetKey(right))
        {
            p1.x++;
        }
        if (p1.magnitude > 0)
        {
            Vector2Int dir = player.index;
            dir += p1;
            if (dir.x >= 1 && dir.x < width - 1 && dir.y >= 1 && dir.y < height - 1)
            {
                Tile t = tiles[dir.x, dir.y];
                if ((player.transform.position - t.transform.position).magnitude < (t.transform.position - player.target.transform.position).magnitude*1.1f)
                {
                    Move(player, dir);
                }
                player.MoveDir = p1;
            }
            else
            {

                player.MoveDir = new Vector2Int();
            }
        }
        else
        {
            player.MoveDir = new Vector2Int();
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayer(Player1, KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D);
        UpdatePlayer(Player2, KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow);
    }
}
