using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CardStruct
{
    public string name;
    public string description;
    public Sprite logo;

    public int cost, number;
    public string type;
    public int actions;
    public int money;
    public int buy;

    public bool isExist
    {
        get
        {
            return number > 0;
        }
    }

    public CardStruct(string name, string description, string logoPath, int cost,
        int number, string type, int money, int actions, int buy)
    {
        string str = "";
        this.name = name;
        this.money = money;
        this.actions = actions;
        this.buy = buy;
        if (money > 0)
        {
            str = str + "+" + money + " денег\n";
        }
        if (actions > 0)
        {
            str = str + "+" + actions + " действий\n";
        }
        if (buy > 0)
        {
            str = str + "+" + buy + " покупок\n";
        }

        this.description = str+description;
        this.cost = cost;
        this.number = number;
        logo = Resources.Load<Sprite>(logoPath);
        this.type = type;
      
    }
}

