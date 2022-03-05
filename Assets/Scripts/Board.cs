using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public Trash trash;
    public PowerUp[] powerUps;
    public GameObject resetMenu;
    public ScoreBoard scoreBoard;
    public GameObject pauseMenu;
    int[,] types;
    Tile[,] tiles;
    public static Board board;
    List<Trash> trashCans = new List<Trash>();

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
        Player1.transform.position = Player1.target.transform.position+offset;
        Player2.transform.position = Player2.target.transform.position + offset;
        for (int i = 1; i < width-1; i++)
        {
            SpawnCustomerOnTile(tiles[i, height - 1]);
        }
        SpawnChoppingBoard(Mathf.FloorToInt(width / 2.0f) - 1, 0, Player1, false);
        SpawnChoppingBoard(Mathf.FloorToInt(width / 2.0f) + 1, 0, Player2, true);
        PlateRack pr = Instantiate(plateRack);
        pr.transform.position = tiles[Mathf.FloorToInt(width / 2.0f), 0].transform.position+ offset;
        scoreBoard.Load();

    }

    void SpawnChoppingBoard(int i, int ii, Player p, bool right)
    {
        ChoppingBoard cb = Instantiate(choppingBoard);
        Tile choppingBoardTile = tiles[i,ii];
        cb.transform.position = choppingBoardTile.transform.position+ offset;
        choppingBoardTile.occupant = cb;
        cb.player = p;
        cb.transform.SetParent(canvas.transform, false);
        Trash trash1 = Instantiate(trash);
        Tile tt = tiles[right ? i + 1 : i - 1, ii];
        trash1.transform.position = tt.transform.position + offset;
        trash1.cb = cb;
        tt.occupant = trash1;
        trashCans.Add(trash1);
    }

    public IEnumerator SpawnCustomer(Tile tile)
    {
        yield return new WaitForSeconds(Random.Range(0, 5.0f));
        Customer c = Instantiate(customer);
        c.transform.SetParent(canvas.transform, false);
        c.target = tile;
        c.transform.position = c.target.transform.position + new Vector3(0, 5, 0);
        c.Load(Random.Range(6.0f,10.0f));
        c.target.occupant = c;
    }

    public void SpawnCustomerOnTile(Tile tile)
    {
        StartCoroutine(SpawnCustomer(tile));
    }

    void SpawnPowerUp(int player)
    {
        int r = Random.Range(0, powerUps.Length / 2);
        PowerUp p = Instantiate(powerUps[powerUps.Length / 2 * player+r]);
        Vector2Int pos = new Vector2Int(Random.Range(1, width - 1), Random.Range(1, height - 1));
        Tile t = tiles[pos.x, pos.y];
        p.transform.position = t.transform.position+offset;
        t.occupant = p;

    }

    public void CustomerDisatisfied()
    {
        Player1.time -= 5;
        Player1.points -= 50;
        Player1.SetPoints();
        Player2.time -= 5;
        Player2.points -= 50;
        Player2.SetPoints();
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
                    int orderAmount = c.TakeOrder(plate);
                    if (orderAmount > 0)
                    {
                        c.angeredBy.Clear();
                        p.time += 5*orderAmount;
                        p.points += 50*orderAmount;
                        p.SetPoints();
                        if (c.GetTimePercent() < 0.30f)
                        {
                            SpawnPowerUp(p.PlayerNum);
                        }
                    }else
                    {
                        if (!c.angeredBy.Contains(p))
                        {
                            c.angeredBy.Add(p);
                        }
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

            }else if (e.ClassType == 4)
            {
                List<Entity> ho = p.DropAll();
                Trash t = e.GetComponent<Trash>();
                int a = t.Empty(ho);
                p.points -= 10*a;
                p.time -= a * 5;
                p.SetPoints();
            }else if (e.ClassType == 6)
            {
                p.PowerUp(e.GetComponent<PowerUp>().PowerUpType);
                Destroy(e.gameObject);
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
                    Check(player, dir);
                    Move(player, dir);
                }
            }
        }
    }

    void ResetPlayer(Player p)
    {
        for (int i = 0; i < trashCans.Count; i++)
        {
            trashCans[i].Empty(p.DropAll());
        }
        p.Reset();
    }

    public void ResetGame()
    {
        scoreBoard.Save();
        ResetPlayer(Player1);
        ResetPlayer(Player2);
        Player1.index = new Vector2Int(1, Mathf.FloorToInt(height / 2.0f));
        Player2.index = new Vector2Int(width - 2, Mathf.FloorToInt(height / 2.0f));
        Player1.target = tiles[Player1.index.x, Player1.index.y];
        Player2.target = tiles[Player2.index.x, Player2.index.y];
        Player1.target.occupant = Player1;
        Player2.target.occupant = Player2;
        Player1.transform.position = Player1.target.transform.position + offset;
        Player2.transform.position = Player2.target.transform.position + offset;
        Customer[] customers = FindObjectsOfType<Customer>();
        for (int i = 0; i < customers.Length; i++)
        {
            Destroy(customers[i].gameObject);
        }
        for (int i = 1; i < width - 1; i++)
        {
            SpawnCustomerOnTile(tiles[i, height - 1]);
        }
        resetMenu.SetActive(false);
    }


    public void PauseGame()
    {
        if (pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(false);
        }else
        {

            pauseMenu.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Player1.dead && Player2.dead && !resetMenu.activeInHierarchy)
        {
            scoreBoard.AddScore(Player1.points);
            scoreBoard.AddScore(Player2.points);
            resetMenu.SetActive(true);
        }
        UpdatePlayer(Player1, KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D);
        UpdatePlayer(Player2, KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow);
    }
}
