using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : Entity
{

    public ChoppingBoard cb;

    void Start()
    {
        ClassType = 4;
    }

    public int Empty(List<Entity> objects)
    {
        int amount = 0;
        for (int i = 0; i < objects.Count; i++)
        {
            Entity e = objects[i];
            if (e.ClassType == 1)
            {
                amount++;
                Destroy(e.gameObject);
            }else if (e.ClassType == 5)
            {
                Plate p = e.GetComponent<Plate>();
                for (int ii = 0; ii < p.vegetables.Count; ii++)
                {
                    amount++;
                }
                Destroy(e.gameObject);
            }
        }
        List<Vegetable> veggies = cb.GetAll();
        for (int i = 0; i < veggies.Count; i++)
        {
            amount++;
            Destroy(veggies[i].gameObject);
        }
        return amount;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
