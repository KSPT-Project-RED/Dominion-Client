using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//структура!!!!
public struct CardStruct
{
    public string name;
    public string description;
    public Sprite logo;

    public int attack, defense;
    public bool canAttack;

    public bool isAlive
    {
        get
        {
            return defense > 0;
        }
    }

    public CardStruct(string name, string description, string logoPath, int attack, int defense) 
    {
        this.name = name;
        this.description = description;
        this.attack = attack;
        this.defense = defense;
        logo = Resources.Load<Sprite>(logoPath);
        canAttack = false;
    }

    public void ChangeAttackState(bool can)
    {
        canAttack = can;
    }

    public void GetDamage(int dmg)
    {
        defense -= dmg;
    }
}

//здесь хранится список всех карт
public static class CardStructManager
{
    public static List<CardStruct> allCardStructs = new List<CardStruct>();
}

public class CardManager : MonoBehaviour
{
    public void Awake()
    {
        CardStructManager.allCardStructs.Add(new CardStruct("1st card", "AAAAA", "Sprites/Cards/cartoon-3591514_640", 3, 7));
        CardStructManager.allCardStructs.Add(new CardStruct("2nd card", "BBBBB", "Sprites/Cards/devil-33699_640", 4, 4));
        CardStructManager.allCardStructs.Add(new CardStruct("3rd card", "CCCCC", "Sprites /Cards/dragon-310237_640", 1, 8));
        CardStructManager.allCardStructs.Add(new CardStruct("4th card", "DDDDD", "Sprites/Cards/frog-3779345_640", 8, 2));
        CardStructManager.allCardStructs.Add(new CardStruct("5th card", "EEEEE", "Sprites/Cards/halloween-4462102_640", 1, 1));

        CardStructManager.allCardStructs.Add(new CardStruct("6th card", "", "Sprites/Cards/monster-158850_640", 3, 7));
        CardStructManager.allCardStructs.Add(new CardStruct("7th card", "", "Sprites/Cards/monster-426995_640", 4, 4));
        CardStructManager.allCardStructs.Add(new CardStruct("8th card", "", "Sprites/Cards/monster-4454504_640", 1, 8));
        CardStructManager.allCardStructs.Add(new CardStruct("9th card", "", "Sprites/Cards/mushroom-576463_640", 8, 2));
        CardStructManager.allCardStructs.Add(new CardStruct("10th card", "", "Sprites/Cards/witch-148439_640", 1, 1));

        CardStructManager.allCardStructs.Add(new CardStruct("11th card", "", "Sprites/Cards/pumpkin-1640465_640", 8, 2));
        CardStructManager.allCardStructs.Add(new CardStruct("12th card", "22222", "Sprites/Cards/pumpkin-1640482_640", 1, 1));
    }

}
