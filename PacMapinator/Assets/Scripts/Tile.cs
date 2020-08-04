using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Wall = 1,
    Food = 2,
    Monster = 3,
    Player = 4,
    EmptyPath = 0
}

public class Tile : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float onMouseHoverAlpha = 0.9f;
    TileType tileType;
    public TileType TileType { get => tileType; }

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
}
