﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{

    public Tile target;
    public Vector2Int index;
    List<Entity> heldObjects = new List<Entity>();

    void Start()
    {
        ClassType = 0;
    }

    public void AddEntity(Entity e)
    {
        e.transform.SetParent(this.transform, false);
        e.transform.localPosition = new Vector3(0.5f, heldObjects.Count/4.0f, -heldObjects.Count/100.0f);
        heldObjects.Add(e);
    }

    // Update is called once per frame
    void Update()
    {
        if ((target.transform.position - transform.position).magnitude > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.position, 0.1f);
        }else
        {
            transform.position = target.transform.position;
        }
    }
}