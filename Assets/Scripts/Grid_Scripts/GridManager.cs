using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GridManager : MonoBehaviour
{
    // Singleton
    public static GridManager Instance;

    // Grid
    private Grid _grid;
    private GridTile[] _gridTiles;
    [Header("Total length of the grid in X and Y directions")]
    [SerializeField] private int _gridSizeX = 21;
    [SerializeField] private int _gridSizeY = 11;
    [SerializeField] private MeshRenderer _gridVisual; // Set in inspector
    [SerializeField] private SpriteRenderer _selectedTileIndicator; // Set in inspector
    //private Vector3 _selectedTilePosition;
    private GridTile _selectedTile;

    // Selected tower
    private Tower _selectedTower;

    // Invalid tiles
    //private Dictionary<Vector3, Tower> _towerTiles = new Dictionary<Vector3, Tower>();
    //private HashSet<Vector3> _invalidTiles = new HashSet<Vector3>();

    // Burning tiles
    [SerializeField] private float _burnThreshold = 3f;
    [SerializeField] private float _burnMultiplier = 1.5f;
    [SerializeField] private float _healMultiplier = 0.75f;
    private List<Vector3> tilesInBurnRange = new List<Vector3>();
    //private List<GridTile> _tilesInRange = new List<GridTile>();
    //private Dictionary<GridTile, float> _burningTiles = new Dictionary<GridTile, float>();

    // Getters
    public GridTile SelectedTile => _selectedTile;

    // Initialization
    private void Awake()
    {
        // Make sure there is only one instance
        if (Instance != null) Destroy(gameObject);
        else Instance = this;

        _grid = GetComponent<Grid>();

        InitializeGrid();
    }
    private void InitializeGrid()
    {
        _gridTiles = new GridTile[_gridSizeX * _gridSizeY];

        int xDist = Mathf.RoundToInt((_gridSizeX - 1) * 0.5f);
        int yDist = Mathf.RoundToInt((_gridSizeY - 1) * 0.5f);

        int counter = 0;
        for (int x = -xDist; x <= xDist; x++)
        {
            for (int y = -yDist; y <= yDist; y++)
            {
                Vector3 position = new Vector3(x, y, 0);
                _gridTiles[counter] = new GridTile(position, _burnThreshold, _burnMultiplier, _healMultiplier);
                counter++;
            }
        }
    }
    public void InitializePathTiles()
    {
        // Loop over every tile
        for (int i = 0; i < _gridTiles.Length; i++)
        {
            // Offset the raycast position so it properly hits the path
            Vector3 raycastPosition = new Vector3(_gridTiles[i].Position.x, _gridTiles[i].Position.y, -10f);

            // Raycast against the path
            if (Physics.Raycast(raycastPosition, Vector3.forward, Mathf.Infinity, LayerMasks.PathMask))
            {
                // The path is over this tile, so mark it as a path tile
                _gridTiles[i].SetTileAsPath(true);
            }
        }
    }

    public void ResetGrid()
    {
        for (int i = 0; i < _gridTiles.Length; i++)
        {
            if (_gridTiles[i].HasTower) Destroy(_gridTiles[i].Tower.gameObject);
            _gridTiles[i].ResetTileInfo();
        }
    }

    private GridTile GetTileFromTilePosition(Vector3 tilePosition) => Array.Find(_gridTiles, tile => tile.Position == tilePosition);
    // Gets the tile from any world point
    public GridTile GetTileFromWorldPosition(Vector3 worldPosition)
    {
        Vector3 tilePosition = _grid.WorldToCell(worldPosition);
        return GetTileFromTilePosition(tilePosition);
    }

    public void CalculateSelectedTile()
    {
        // Turn the mouse screen position into a world space point
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0f;

        // Get the grid position of the mouse
        Vector3 gridPosition = _grid.WorldToCell(worldPosition);

        // Find the tile for that grid position and set it as the selected tile
        _selectedTile = GetTileFromTilePosition(gridPosition);

        // Set the selected tile indicator
        UpdateTileIndicator(_selectedTile);
    }
    private void UpdateTileIndicator(GridTile tile)
    {
        _selectedTileIndicator.transform.position = tile.Position;
        if (tile.IsValidTile()) _selectedTileIndicator.color = new Color(1f, 1f, 1f, 0.4f); // Transparent white
        else _selectedTileIndicator.color = new Color(1f, 0f, 0f, 0.4f); // Transparent red
    }
    public void ClickOnGrid()
    {
        //Debug.Log("Clicked on grid!");

        // Hide the range of the previous selected tower
        _selectedTower?.ToggleRangeVisual(false);

        CalculateSelectedTile();
        //Debug.Log($"Selected tile: {_selectedTilePosition}");

        _selectedTower = _selectedTile.Tower;

        if (_selectedTower != null)
        {
            // Disable the tower panels
            TextUIManager.Instance.ToggleTowerPanels(false);

            // Enable the tower info panel
            TextUIManager.Instance.ToggleTowerInfo(true);

            //Debug.Log(tower.name);
            TextUIManager.Instance.UpdateTowerInfo(_selectedTower);

            // Show the tower's range
            _selectedTower.ToggleRangeVisual(true);
        }
        else
        {
            // Disable the tower info panel
            TextUIManager.Instance.ToggleTowerInfo(false);

            // Enable the tower panels
            TextUIManager.Instance.ToggleTowerPanels(true);
        }
    }




    private void Update()
    {
        // Loop over every tile and update them
        foreach (GridTile tile in _gridTiles)
        {
            // Check if this tile is in burn range
            bool inBurnRange = tilesInBurnRange.Contains(tile.Position) ? true : false;

            // Update the tiles
            tile.UpdateTile(inBurnRange);
            
            //DebugExtension.DebugCircle(tile.Position, -Vector3.forward, Color.white, 0.5f);
        }

        // Clear the tiles in burn range list
        tilesInBurnRange.Clear();

        // Maybe do this in LateUpdate() to make sure all the enemies have been calculated first?

        // Update the burning tiles
        // List<Vector3> tilesToRemove = new List<Vector3>();
        // GridTile[] burnedTiles = _burningTiles.Keys.ToArray();

        // foreach (GridTile tile in burnedTiles)
        // {
        //     if (!_tilesInRange.Contains(tile))
        //     {
        //         // If the tile is no longer being burned this frame
        //         // Remove from its timer
        //         _burningTiles[tile] -= Time.deltaTime;

        //         // If the timer drops below the threshold, it is no longer invalid


        //         // If the timer drops below 0, it is no longer burning
        //         if (_burningTiles[tile] <= 0f)
        //         {
        //             //_burningTiles.Remove(tile);
        //             tilesToRemove.Add(tile);
        //         }
        //     }

        //     // Highlight the tiles
        //     DebugExtension.DebugCircle(tile, -Vector3.forward, Color.yellow, 0.5f);
        // }

        // // Remove the non-burnt tiles
        // foreach (Vector3 tile in tilesToRemove) _burningTiles.Remove(tile);

        // // Clear the tiles in range list
        // _tilesInRange.Clear();
    }









    // public void AddTowerToSelectedTile(Tower tower)
    // {
    //     _selectedTile.HasTower = true;
    //     _selectedTile.Tower = tower;
    //     //Debug.Log($"Added tower to tile: {_selectedTile.Position}");
    // }
    // public void RemoveTowerFromSelectedTile(Tower tower)
    // {
    //     _selectedTile.HasTower = false;
    //     _selectedTile.Tower = null;

    //     // // Remove from towers list
    //     // if (!_towerTiles.Remove(_selectedTilePosition))
    //     // {
    //     //     Debug.LogWarning($"Could not find a tower at {_selectedTilePosition}");
    //     // }

    //     // // Set the tile as valid again
    //     // _invalidTiles.Remove(_selectedTilePosition);

    //     // Destroy the tower
    //     Destroy(tower.gameObject);
    // }
    public void UpdateSelectedTowerTargetMode()
    {
        _selectedTower.ChangeTargetMode();

        // Update the UI again
        TextUIManager.Instance.UpdateTowerInfo(_selectedTower);
    }
    public void SellSelectedTower()
    {
        // Give orbs to player
        GameManager.Instance.AddOrbs(_selectedTower.SellValue);

        // Remove the tower
        //RemoveTowerFromSelectedTile(_selectedTower);
        _selectedTile.RemoveTowerFromTile();
        Destroy(_selectedTower.gameObject);

        // Disable the tower info panel
        TextUIManager.Instance.ToggleTowerInfo(false);

        // Enable the tower panels
        TextUIManager.Instance.ToggleTowerPanels(true);

        _selectedTower = null;
    }


    public Vector3 GetNearestTile(Vector3 position) => Vector3Int.RoundToInt(position);
    public void BurnTilesInRange(Vector3 position, float radius)
    {
        int minX = Mathf.RoundToInt(position.x - radius);
        int maxX =  Mathf.RoundToInt(position.x + radius);
        int minY = Mathf.RoundToInt(position.y - radius);
        int maxY = Mathf.RoundToInt(position.y + radius);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector3 tilePosition = new Vector3(x, y, 0);
                if (Vector3.Distance(position, tilePosition) > radius) continue;

                if (!tilesInBurnRange.Contains(tilePosition))
                {
                    tilesInBurnRange.Add(tilePosition);
                }
            }
        }


        // _tilesInRange = GetTilesInRange(position, radius);

        // foreach (Vector3 tile in _tilesInRange)
        // {
        //     //DebugExtension.DebugCircle(tile, -Vector3.forward, Color.yellow, 0.5f);

        //     if (_burningTiles.ContainsKey(tile))
        //     {
        //         // If the tile is already burning, add to the timer
        //         _burningTiles[tile] += Time.deltaTime;

        //         // If the timer goes above the burn threshold, make it invalid
        //         if (_burningTiles[tile] >= _burnThreshold)
        //         {
        //             //BurnTile(tile);
        //         }
        //     }
        //     else
        //     {
        //         // This tile is starting to burn
        //         _burningTiles.Add(tile, Time.deltaTime);
        //     }
        // }
    }
    private List<Vector3> GetTilesInRange(Vector3 position, float radius)
    {
        List<Vector3> tilesPositionsInRange = new List<Vector3>();



        return tilesPositionsInRange;
    }

    public void HighlightGridVisual(bool value) => _gridVisual.enabled = value;
    public void HighlightTileSelector(bool value) => _selectedTileIndicator.enabled = value;

}