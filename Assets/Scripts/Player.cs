using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{

    public float time;
    public float points;
    public Text pointsText;
    public Text timeText;
    public Tile target;
    public Vector2Int index;
    List<Entity> heldObjects = new List<Entity>();
    float speed = 5;
    public bool up;
    public bool down;
    public bool right;
    public bool left;

    void Start()
    {
        ClassType = 0;
    }

    public void AddEntity(Entity e)
    {
        e.transform.SetParent(this.transform, false);
        e.transform.localPosition = new Vector3(0.5f, heldObjects.Count/4.0f, -heldObjects.Count/16.0f);
        heldObjects.Add(e);
    }

    public int GetHoldCount()
    {
        return heldObjects.Count;
    }

    public Vegetable GetVegetable()
    {
        if (heldObjects.Count > 0)
        {
            Vegetable v = heldObjects[0].GetComponent<Vegetable>();
            if (v != null)
            {
                heldObjects.RemoveAt(0);
                return v;
            }
        }
        return null;
    }

    public Plate GetPlate()
    {
        if (heldObjects.Count > 0)
        {
            Plate v = heldObjects[0].GetComponent<Plate>();
            if (v != null)
            {
                heldObjects.RemoveAt(0);
                return v;
            }
        }
        return null;
    }

    public bool CheckIngredients(List<int> ingredients)
    {
        return false;
    }

    public void SetPoints()
    {
        pointsText.text = "" + points + "  -  Points";
    }

    public Vector2Int MoveDir;
    float timer = 0;
    bool dead = false;
    // Update is called once per frame
    void Update()
    {
        if (dead)
        {
            return;
        }
        timer += Time.deltaTime;
        float f = time - timer;
        f = Mathf.Round(f);
        timeText.text = "" + f + "  -  Time";
        if (timer >= time)
        {
            dead = true;
        }
        Vector3 v = target.transform.position + new Vector3(0, 0, -1.0f / 16.0f) - transform.position;
        if (v.magnitude > speed * Time.deltaTime * 0.9f)
        {
            transform.position += v.normalized * speed * Time.deltaTime;
        }
        else
        {
            transform.position = target.transform.position + new Vector3(0,0,-1.0f/16.0f);
        }
    }
}
