using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoppingBoard : Entity
{

    public Slider slider;
    public Player player;
    List<Vegetable> vegetables = new List<Vegetable>();
    bool chopping = false;
    float timer = 0;
    float time = 0;

    void Start()
    {
        ClassType = 3;
    }

    public List<Vegetable> GetAll()
    {
        List<Vegetable> veggies = new List<Vegetable>();
        veggies.AddRange(vegetables);
        vegetables.Clear();
        return veggies;
    }

    public bool CheckPlayer(Player player)
    {
        return (player == this.player);
    }

    public void AddVegetable(Vegetable v)
    {
        vegetables.Add(v);
        v.transform.parent = null;
        PositionVegetables();
        time += 3;
    }

    void PositionVegetables()
    {
        float positionScaler = 0.25f;
        float objectScaler = 0.5f;
        for (int i = 0; i < vegetables.Count; i++)
        {
            Vegetable v = vegetables[i];
            v.transform.position = transform.position + new Vector3((-(vegetables.Count - 1) / 2.0f) * positionScaler + i * positionScaler, 0, -1.0f/16.0f);
            v.transform.localScale = new Vector3(objectScaler, objectScaler, objectScaler);
        }
    }

    public void Chop()
    {
        if (vegetables.Count > 0)
        {
            chopping = true;
        }
    }

    void CreateSalad()
    {
        Plate plate = Instantiate(Board.board.plateRack.plate);
        for (int i = 0; i < vegetables.Count; i++)
        {
            plate.AddVegetable(vegetables[i]);
        }
        vegetables.Clear();
        player.AddEntity(plate);
    }



    // Update is called once per frame
    void Update()
    {
        if (chopping && (player.transform.position - transform.position).magnitude < 1.1f)
        {
            timer += Time.deltaTime;
            slider.value = 1.0f - (timer > time ? time : timer) / time;
            if (timer >= time)
            {
                time = 0;
                timer = 0;
                chopping = false;
                slider.value = 1;
                CreateSalad();
            }
        }
    }
}
