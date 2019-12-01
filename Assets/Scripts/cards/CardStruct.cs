using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CardStruct
{
    public string name;
    public string description;
    public Sprite logo;

    public int cost, number;

    public bool isExist
    {
        get
        {
            return number > 0;
        }
    }

    public CardStruct(string name, string description, string logoPath, int cost, int number)
    {
        this.name = name;
        this.description = description;
        this.cost = cost;
        this.number = number;
        logo = Resources.Load<Sprite>(logoPath);
    }
}

