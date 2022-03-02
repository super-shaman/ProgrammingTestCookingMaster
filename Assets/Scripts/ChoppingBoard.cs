using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppingBoard : Entity
{

    List<Vegetable> vegetables = new List<Vegetable>();

    void Start()
    {
        ClassType = 3;
    }

    public void AddVegetable(Vegetable v)
    {
        vegetables.Add(v);
        v.transform.parent = null;
        PositionVegetables();
    }

    void PositionVegetables()
    {
        float positionScaler = 0.25f;
        float objectScaler = 0.5f;
        for (int i = 0; i < vegetables.Count; i++)
        {
            Vegetable v = vegetables[i];
            v.transform.position = transform.position + new Vector3((-(vegetables.Count - 1) / 2.0f) * positionScaler + i * positionScaler, 0, -1);
            v.transform.localScale = new Vector3(objectScaler, objectScaler, objectScaler);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
