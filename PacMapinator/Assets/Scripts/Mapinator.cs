using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;


public class Mapinator : MonoBehaviour
{
    [SerializeField] int row = 3;
    [SerializeField] int column = 3;
    public int Row 
    { 
        get => row; 
        set 
        { 
            if (row != value)
                row = value; 
        } 
    }
    public int Column 
    { 
        get => column; 
        set 
        { 
            if (column != value)
                column = value; 
        } 
    }

    [SerializeField] GameObject Prefab;
    //This will contain the cube prefabs
    List<GameObject> TilesPool = new List<GameObject>();

    Sprite selectingTile;
    public Sprite SelectingTile { get => selectingTile; set { selectingTile = value; } }
    TileType selectingTileType;
    public TileType SelectingTileType { get => selectingTileType; set { selectingTileType = value; } }

    [SerializeField] GameObject SimpleDialog;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            this.CreateMap();
        }
    }

    public void CreateMap()
    {
        if(TilesPool.Count < column*row)
        {
            int more = column * row - TilesPool.Count;
            for (int i = 0; i  < more; i++)
            {
                GameObject tile = Instantiate(Prefab, Vector3.zero, Quaternion.identity, transform);
                tile.SetActive(false);
                this.TilesPool.Add(tile);
            }
        }

        foreach (var tile in TilesPool)
        {
            tile.SetActive(false);
        }

        int k = 0;
        for(int i=0; i<this.row; i++)
        {
            for(int j=0; j<this.column; j++)
            {
                GameObject tile = this.TilesPool[k];
                tile.SetActive(true);
                tile.transform.position = new Vector3(j, i);
                k++;
            }
        }
    }

    public void SetRow(TMP_InputField r)
    {
        if(r.text.Length != 0)
        {
            int.TryParse(r.text, out this.row);
           // Debug.Log($"Row: {this.Row}");
        }
    }

    public void SetColumn(TMP_InputField c)
    {
        if(c.text.Length != 0)
        {
            this.Column = int.Parse(c.text);
            //Debug.Log($"Col: {this.Column}");
        }
    }

    public void SetTileType(int tileType)
    {
        selectingTileType = (TileType)tileType;
    }

    public void Export(string path)
    {
        using(StreamWriter stream = new StreamWriter(path, false, Encoding.ASCII))
        {
            string sizeOfMap = $"{this.row} {this.column}";
            stream.WriteLine(sizeOfMap);

            string playerPos = "";

            int k = 0;
            for(int i=0; i<this.row; i++)
            {
                string r = "";
                for(int j=0; j<this.column; j++)
                {
                    Tile tile = this.TilesPool[k].GetComponentInChildren<Tile>();
                    if(tile.TileType == TileType.Player)
                    {
                        playerPos += tile.gameObject.transform.position.x + " " + tile.gameObject.transform.position.y;

                        r += (int)TileType.EmptyPath + " ";
                    }
                    else
                    {
                        r += (int)tile.TileType + " ";
                    }

                    k++;
                }
                stream.WriteLine(r);
            }

            stream.WriteLine(playerPos);
        }
        //Debug.Log($"Export to {path}");
        PopupDialog($"Export to {Path.GetFullPath(path)}");
    }

    public void Export(TMP_InputField path)
    {
        this.Export(path.text);
    }

    public void PopupDialog(string msg)
    {
        var mesGUI = SimpleDialog.GetComponentInChildren<TextMeshProUGUI>();
        mesGUI.text = msg;
        this.SimpleDialog.SetActive(true);
    }
}
