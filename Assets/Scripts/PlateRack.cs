using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateRack : MonoBehaviour
{

    public Plate plate;

    void Start()
    {
        for (int i = 0; i < 8; i++)
        {
            Plate p = Instantiate(plate);
            p.transform.SetParent(transform, false);
            p.transform.localPosition = new Vector3(0, i / 16.0f, -i / 16.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
