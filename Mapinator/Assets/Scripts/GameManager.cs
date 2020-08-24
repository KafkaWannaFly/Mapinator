using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static public GameManager Singleton;
    [SerializeField] Mapinator mapinator;

    public Sprite SelectingSprite { get => mapinator.SelectingTile; }
    public TileType SelectingTileType { get => mapinator.SelectingTileType;  }

    private void Awake()
    {
        if(GameManager.Singleton == null)
        {
            GameManager.Singleton = this;
        }
    }
}
