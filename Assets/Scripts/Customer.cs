﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Customer : Entity
{

    public Slider slider;
    public GameObject order;
    public Vector3 offset;
    public Vector3 ExitPosition;
    public Tile target;
    List<int> OrderedIngredients = new List<int>();
    float time;
    float timer = 0;
    bool exit = false;

    public void Load(float time)
    {
        int range = Random.Range(2, 4);
        for (int i = 0; i < range; i++)
        {
            OrderedIngredients.Add(Random.Range(0, Board.board.Vegetables.Length));
        }
        float positionScaler = 0.25f;
        float objectScaler = 0.5f;
        for (int i = 0; i < range; i++)
        {
            int index = OrderedIngredients[i];
            Vegetable v = Instantiate(Board.board.Vegetables[index]);
            v.transform.SetParent(order.transform, false);
            v.transform.localPosition = new Vector3((-(range-1) / 2.0f)*positionScaler+i*positionScaler, 0, -1);
            v.transform.localScale = new Vector3(objectScaler, objectScaler, objectScaler);
        }
        this.time = time;
    }

    public List<int> GetIngredients()
    {
        return OrderedIngredients;
    }

    void Start()
    {
        ClassType = 2;
    }

    public IEnumerator Despawn()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    private void Exit()
    {
        float dist = (ExitPosition - transform.position).magnitude;
        if (dist > 0.02f)
        {
            transform.position = Vector3.Lerp(transform.position, ExitPosition, (1.0f / (dist < 1 ? 1 : dist) * 0.02f));
        }
        else
        {
            transform.position = ExitPosition;
        }

    }

    void Update()
    {
        if (exit)
        {
            Exit();
            return;
        }
        timer += Time.deltaTime;
        slider.value = 1 - (timer > time ? time : timer) / time;
        if (timer >= time)
        {
            target.occupant = null;
            Board.board.SpawnCustomerOnTile(target);
            exit = true;
            ExitPosition = transform.position + new Vector3(0, 5, 0);
            StartCoroutine(Despawn());

        }
        float dist = (target.transform.position+offset - transform.position).magnitude;
        if (dist > 0.02f)
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.position+offset, (1.0f / (dist < 1 ? 1 : dist) * 0.05f));
        }
        else
        {
            transform.position = target.transform.position+offset;
        }
    }
}