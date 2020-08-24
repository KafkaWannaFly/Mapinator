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

    [SerializeField] GameObject TilePrefab;
    [SerializeField] Sprite defaultSprite;
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
                GameObject tile = Instantiate(TilePrefab, Vector3.zero, Quaternion.identity, transform);
                tile.SetActive(false);
                this.TilesPool.Add(tile);
            }
        }

        foreach (var tile in TilesPool)
        {
            tile.SetActive(false);

            var _tile = tile.GetComponent<Tile>();
            if(_tile)
            {
                _tile.SetSprite(this.defaultSprite);
                _tile.TileType = TileType.EmptyPath;
            }
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

    /// <summary>
    /// Prepare content before export txt
    /// </summary>
    /// <returns></returns>
    List<List<string>> TileToText()
    {
        List<List<string>> mapOutput = new List<List<string>>();
        
        for (int i = 0; i < this.row; i++)
        {
            mapOutput.Add(new List<string>(new string[this.column]));
        }

        int k = 0;
        for (int i = this.row - 1; i >= 0; i--)
        {
            for (int j = 0; j < this.column; j++)
            {
                Tile tile = this.TilesPool[k].GetComponentInChildren<Tile>();
                if(tile.TileType == TileType.Agent)
                {
                    mapOutput[i][j] += Tile.GetLetter(tile.TileType);
                }
                else if(tile.TileType == TileType.Gold)
                {
                    mapOutput[i][j] += Tile.GetLetter(tile.TileType);
                }
                else if(tile.TileType == TileType.EmptyPath)
                {
                    if(String.IsNullOrEmpty(mapOutput[i][j]))
                    {
                        mapOutput[i][j] = Tile.GetLetter(tile.TileType);
                    }
                }
                else if (tile.TileType == TileType.Wumpus)
                {
                    mapOutput[i][j] += Tile.GetLetter(tile.TileType);
                    //Near by is Stench
                    if(i + 1 < row)
                    {
                        //Remove empty state
                        if(mapOutput[i + 1][j] == Tile.GetLetter(TileType.EmptyPath) || String.IsNullOrEmpty(mapOutput[i + 1][j]))
                        {
                            mapOutput[i + 1][j] = Tile.GetLetter(TileType.Stench);
                        }
                        else
                        {
                            //Avoid duplicate
                            if(!mapOutput[i + 1][j].Contains(Tile.GetLetter(TileType.Stench)))
                            {
                                mapOutput[i + 1][j] += Tile.GetLetter(TileType.Stench);
                            }
                        }
                    }
                    if(i - 1 >= 0)
                    {
                        if(mapOutput[i - 1][j] == Tile.GetLetter(TileType.EmptyPath) || String.IsNullOrEmpty(mapOutput[i - 1][j]))
                        {
                            mapOutput[i - 1][j] = Tile.GetLetter(TileType.Stench);
                        }
                        else
                        {
                            if(!mapOutput[i - 1][j].Contains(Tile.GetLetter(TileType.Stench)))
                            {
                                mapOutput[i - 1][j] += Tile.GetLetter(TileType.Stench);
                            }
                        }
                    }
                    if(j + 1 < column)
                    {
                        if(mapOutput[i][j + 1] == Tile.GetLetter(TileType.EmptyPath) || String.IsNullOrEmpty(mapOutput[i][j + 1]))
                        {
                            mapOutput[i][j + 1] = Tile.GetLetter(TileType.Stench);
                        }
                        else
                        {
                            if(!mapOutput[i][j + 1].Contains(Tile.GetLetter(TileType.Stench)))
                            {
                                mapOutput[i][j + 1] += Tile.GetLetter(TileType.Stench);
                            }
                        }
                    }
                    if(j - 1 >= 0)
                    {
                        if(mapOutput[i][j - 1] == Tile.GetLetter(TileType.EmptyPath) || String.IsNullOrEmpty(mapOutput[i][j - 1]))
                        {
                            mapOutput[i][j - 1] = Tile.GetLetter(TileType.Stench);
                        }
                        else
                        {
                            if(!mapOutput[i][j - 1].Contains(Tile.GetLetter(TileType.Stench)))
                            {
                                mapOutput[i][j - 1] += Tile.GetLetter(TileType.Stench);
                            }
                        }
                    }
                }
                else if(tile.TileType == TileType.Pit)
                {
                    mapOutput[i][j] += Tile.GetLetter(tile.TileType);
                    // Near by is Breeze
                    if (i + 1 < row)
                    {
                        //Remove empty state
                        if (mapOutput[i + 1][j] == Tile.GetLetter(TileType.EmptyPath) || String.IsNullOrEmpty(mapOutput[i + 1][j]))
                        {
                            mapOutput[i + 1][j] = Tile.GetLetter(TileType.Breeze);
                        }
                        else
                        {
                            //Avoid duplicate
                            if (!mapOutput[i + 1][j].Contains(Tile.GetLetter(TileType.Breeze)))
                            {
                                mapOutput[i + 1][j] += Tile.GetLetter(TileType.Breeze);
                            }
                        }
                    }
                    if (i - 1 >= 0)
                    {
                        if (mapOutput[i - 1][j] == Tile.GetLetter(TileType.EmptyPath) || String.IsNullOrEmpty(mapOutput[i - 1][j]))
                        {
                            mapOutput[i - 1][j] = Tile.GetLetter(TileType.Breeze);
                        }
                        else
                        {
                            if (!mapOutput[i - 1][j].Contains(Tile.GetLetter(TileType.Breeze)))
                            {
                                mapOutput[i - 1][j] += Tile.GetLetter(TileType.Breeze);
                            }
                        }
                    }
                    if (j + 1 < column)
                    {
                        if (mapOutput[i][j + 1] == Tile.GetLetter(TileType.EmptyPath) || String.IsNullOrEmpty(mapOutput[i][j + 1]))
                        {
                            mapOutput[i][j + 1] = Tile.GetLetter(TileType.Breeze);
                        }
                        else
                        {
                            if (!mapOutput[i][j + 1].Contains(Tile.GetLetter(TileType.Breeze)))
                            {
                                mapOutput[i][j + 1] += Tile.GetLetter(TileType.Breeze);
                            }
                        }
                    }
                    if (j - 1 >= 0)
                    {
                        if (mapOutput[i][j - 1] == Tile.GetLetter(TileType.EmptyPath) || String.IsNullOrEmpty(mapOutput[i][j - 1]))
                        {
                            mapOutput[i][j - 1] = Tile.GetLetter(TileType.Breeze);
                        }
                        else
                        {
                            if (!mapOutput[i][j - 1].Contains(Tile.GetLetter(TileType.Breeze)))
                            {
                                mapOutput[i][j - 1] += Tile.GetLetter(TileType.Breeze);
                            }
                        }
                    }
                }

                k++;
            }
        }
        return mapOutput;
    }

    /// <summary>
    /// Use in Wumpus project
    /// </summary>
    /// <param name="path"></param>
    void ExportForWumpus(string path)
    {
        try
        {
            List<List<string>> mapOutput = this.TileToText();
            using (StreamWriter stream = new StreamWriter(path, false, Encoding.ASCII))
            {
                //Map should be square
                string sizeOfMap = $"{this.row}";
                stream.WriteLine(sizeOfMap);

                for(int i=0; i<row; i++)
                {
                    string tobeWriten = "";
                    for(int j=0; j<column; j++)
                    {
                        tobeWriten += mapOutput[i][j];
                        if(j != column - 1)
                        {
                            tobeWriten += ".";
                        }
                    }
                    stream.WriteLine(tobeWriten);
                }
            }
            PopupDialog($"Export to: {Path.GetFullPath(path)}");
        }
        catch (Exception e)
        {
            PopupDialog(e.Message);
        }
    }

    public void ExportForWumpus(TMP_InputField path)
    {
        this.ExportForWumpus(path.text);
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
