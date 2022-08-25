using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;


public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    [SerializeField] private Transform _cam;
    private Dictionary<Vector2, Tile> _tiles;

    public Texture2D map;
    public ColorToPrefab[] colorMappings;
    private void Awake()
    {
        Instance = this;
    }

    public void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2, Tile>();

        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                GenerateTile(x, y);           
            }
        }
        LinkTiles();

        _cam.transform.position = new Vector3((float)map.width/2 -0.5f, (float)map.height/2 -0.5f,-10);

        GameManager.Instance.ChangeState(GameState.SpawnHeroes);
    }

    void GenerateTile(int x, int y)
    {
        Color pixelColor = map.GetPixel(x, y);

        if (pixelColor.a == 0)
        {
            //pixel is transparent so ignore, safecase
            return;
        }

        foreach (ColorToPrefab colorMapping in colorMappings)
        {
            if (colorMapping.color.Equals(pixelColor))
            {
                Vector2 position = new Vector2(x, y);
                var spawnedTile = Instantiate(colorMapping.prefab, position, Quaternion.identity, transform);
                spawnedTile.tileX = x;
                spawnedTile.tileY = y;
                spawnedTile.name = $"Tile {x} {y}";
                _tiles[position] = spawnedTile;

            }
        }
    }

    //TODO: Is this method necessary or can the grid manager know all of the references instead of the tile???
    private void LinkTiles()
    {
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                Vector2 position = new Vector2(x, y);
                if (_tiles.TryGetValue(position, out Tile currTile))
                {
                    Tile currTileTop = GetTileAtPosition(new Vector2(x,y+1));
                    currTile.tileTop = currTileTop;
                    if (currTileTop != null) currTile.adjacentTiles.Add(currTileTop);


                    Tile currTileBot = GetTileAtPosition(new Vector2(x,y-1));
                    currTile.tileBottom = currTileBot;
                    if (currTileBot != null) currTile.adjacentTiles.Add(currTileBot);

                    Tile currTileRight = GetTileAtPosition(new Vector2(x+1,y));
                    currTile.tileRight = currTileRight;
                    if (currTileRight != null) currTile.adjacentTiles.Add(currTileRight);

                    Tile currTileLeft = GetTileAtPosition(new Vector2(x-1,y));
                    currTile.tileLeft = currTileLeft;
                    if (currTileLeft != null) currTile.adjacentTiles.Add(currTileLeft);
                }
                else
                {
                    Debug.Log("Null tile reference during linking");
                }
            }
        }
    }

    public Tile GetHeroSpawnTile()
    {
        //random tile on the left of the map
        return _tiles.Where(tag => tag.Key.x < map.width / 2 && tag.Value.Walkable).OrderBy(tag => Random.value).First().Value;
    }

    public Tile GetEnemySpawnTile()
    {
        //random tile on the right of the map
        return _tiles.Where(tag => tag.Key.x > map.width / 2 && tag.Value.Walkable).OrderBy(tag => Random.value).First().Value;
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }
    
    //Returns a list of tiles that can be traveled to given a start tile and movespeed of a selected unit
    //Modified from: https://github.com/Angus-Fan/TurnBasedStrategyGame/blob/master/tileMapScript.cs
    public void ShowMovableTiles(Tile startTile, int moveSpeed, int attRange)
    {
        HashSet<Tile> totalMovableTiles = GetMovableTiles(startTile, moveSpeed);
        HashSet<Tile> totalAttackableTiles = GetAttackableTiles(attRange, totalMovableTiles);
        totalAttackableTiles.ExceptWith(totalMovableTiles);

        //Display movable tile overlay
        foreach (Tile t in totalMovableTiles)
        {
            t.ShowMovable();
        }
        foreach (Tile t in totalAttackableTiles)
        {
            t.ShowAttackable();
        }
    }

    //Returns a hash of all tiles given a start tile and movespeed
    public HashSet<Tile> GetMovableTiles(Tile startTile, int moveSpeed)
    {
        int[,] _cost = new int[map.width, map.height];
        HashSet<Tile> UIHighlight = new HashSet<Tile>();
        HashSet<Tile> finalMovementHighlight = new HashSet<Tile>();

        //Add start tile to final
        finalMovementHighlight.Add(startTile);
        _cost[startTile.tileX, startTile.tileY] = 500;

        //Get list of tiles adjacent to start where one move is possible
        foreach (Tile t in startTile.adjacentTiles)
        {
            _cost[t.tileX, t.tileY] = t.moveCost;
            if (moveSpeed - _cost[t.tileX, t.tileY] >= 0) UIHighlight.Add(t);
        }

        //Add the above list to final list
        finalMovementHighlight.UnionWith(UIHighlight);

        //While there are tiles to iterate through...
        while (UIHighlight.Count != 0)
        {
            //Find the lowest cost tile in the set
            Tile minTile = startTile;
            foreach (Tile t in UIHighlight)
            {
                if (_cost[t.tileX, t.tileY] < _cost[minTile.tileX, minTile.tileY]) minTile = t;
            }

            //Look at its neighbor and add it to the list if it's cost + the cost it took to get there is less than the movement speed
            foreach (Tile neighbor in minTile.adjacentTiles)
            {
                //Update cost to be lowest possible and add it to final if it hasn't already been added
                if (_cost[neighbor.tileX, neighbor.tileY] == 0) _cost[neighbor.tileX, neighbor.tileY] = neighbor.moveCost + _cost[minTile.tileX, minTile.tileY];
                if (moveSpeed - _cost[neighbor.tileX, neighbor.tileY] >= 0 && !finalMovementHighlight.Contains(neighbor))
                {
                    UIHighlight.Add(neighbor);
                    finalMovementHighlight.Add(neighbor);
                }
            }

            UIHighlight.Remove(minTile);
        }
        return finalMovementHighlight;
    }

    //Returns a HashSet of all tiles that can be attacked from the given position
    public HashSet<Tile> GetAttackableTiles(int attRange, HashSet<Tile> finalMovementHighlight)
    {
        //Submethod for highlighting attackable tiles
        HashSet<Tile> tempNeighbourHash = new HashSet<Tile>();
        HashSet<Tile> neighbourHash;
        HashSet<Tile> seenTiles = new HashSet<Tile>();
        HashSet<Tile> finalAttackableHighlight = new HashSet<Tile>();
        foreach (Tile n in finalMovementHighlight)
        {
            neighbourHash = new HashSet<Tile>();
            neighbourHash.Add(n);
            for (int i = 0; i < attRange; i++)
            {
                foreach (Tile t in neighbourHash)
                {
                    foreach (Tile tn in t.adjacentTiles)
                    {
                        tempNeighbourHash.Add(tn);
                    }
                }

                neighbourHash = tempNeighbourHash;
                tempNeighbourHash = new HashSet<Tile>();
                if (i < attRange - 1)
                {
                    seenTiles.UnionWith(neighbourHash);
                }

            }
            neighbourHash.ExceptWith(seenTiles);
            seenTiles = new HashSet<Tile>();
            finalAttackableHighlight.UnionWith(neighbourHash);
        }
        return finalAttackableHighlight;
    }

    //From the selected enemy, find the first movable tile of the hero 
    public void MoveInRange()
    {
        BaseEnemy target = UnitManager.Instance.SelectedEnemy;
        HashSet<Tile> startTile = new HashSet<Tile> { target.OccupiedTile };
        HashSet<Tile> movable = GetAttackableTiles(UnitManager.Instance.SelectedHero.attackRange, startTile);
        movable.ExceptWith(startTile);
        foreach(Tile t in movable)
        {
            if (t._movableHighlight.activeSelf)
            {
                t.SetUnit(UnitManager.Instance.SelectedHero);
                return;
            }
        }

    }

    public void ClearMovementOverlay()
    {
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                _tiles[new Vector2(x, y)].HideHighlights();
            }
        }
    }
}