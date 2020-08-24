using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    EmptyPath = 0,
    Wall = 1,
    Food = 2,
    Monster = 3,
    Player = 4,
    Gold,
    Pit,
    Wumpus,
    Agent,
    Stench,
    Breeze
}


public class Tile : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float onMouseHoverAlpha = 0.9f;
    TileType tileType;
    public TileType TileType { get => tileType; set => tileType = value; }

    private void OnMouseEnter()
    {
        SetAlpha(onMouseHoverAlpha);
    }

    private void OnMouseExit()
    {
        SetAlpha(1);
    }

    private void OnMouseDown()
    {
        SetSprite(GameManager.Singleton.SelectingSprite);
    }

    private void OnMouseDrag()
    {
        SetSprite(GameManager.Singleton.SelectingSprite);
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        tileType = GameManager.Singleton.SelectingTileType;
    }

    public void SetAlpha(float a)
    {
        Color color = spriteRenderer.color;
        color.a = a;
        spriteRenderer.color = color;
    }

    /// <summary>
    /// Use for Wumpus World project
    /// </summary>
    /// <param name="tileType"></param>
    /// <returns></returns>
    static public string GetLetter(TileType tileType)
    {
        if(tileType == TileType.EmptyPath)
        {
            return "-";
        }
        else if(tileType == TileType.Wumpus)
        {
            return "W";
        }
        else if(tileType == TileType.Pit)
        {
            return "P";
        }
        else if (tileType == TileType.Gold)
        {
            return "G";
        }
        else if(tileType == TileType.Agent)
        {
            return "A";
        }
        else if(tileType == TileType.Breeze)
        {
            return "B";
        }
        else if(tileType == TileType.Stench)
        {
            return "S";
        }
        else
        {
            throw new Exception("Unregconized TileType!");
        }
    }
}
