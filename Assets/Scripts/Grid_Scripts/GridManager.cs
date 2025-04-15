using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    // Singleton
    public static GridManager Instance;

    // Grid
    private Grid _grid;
    [SerializeField] private MeshRenderer _gridVisual; // Set in inspector
    [SerializeField] private SpriteRenderer _selectedTileIndicator; // Set in inspector
    private Vector3 _selectedTilePosition;

    // Selected tower
    private Tower _selectedTower;

    // Invalid tiles
    private Dictionary<Vector3, Tower> _towerTiles = new Dictionary<Vector3, Tower>();
    private HashSet<Vector3> _invalidTiles = new HashSet<Vector3>();


    //[SerializeField] private int width, height; // Set in inspector
    //[SerializeField] private Tile tilePrefab; // Set in inspector
    //[SerializeField] private Transform mainCamera; // Set in inspector

    //private Dictionary<Vector2, Tile> tiles;

    // Getters
    public Vector3 SelectedTilePosition => _selectedTilePosition;


    private void Awake()
    {
        // Make sure there is only one instance
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _grid = GetComponent<Grid>();

        DontDestroyOnLoad(gameObject);
    }

    public void CheckForPathTiles(int pathCheckDistance)
    {
        // Loop over every tile within path check distance
        for (int x = -pathCheckDistance; x <= pathCheckDistance; x++)
        {
            for (int y = -pathCheckDistance; y <= pathCheckDistance; y++)
            {
                Vector3 tilePosition = new Vector3(x, y, -10f);

                if (Physics.Raycast(tilePosition, Vector3.forward, Mathf.Infinity, LayerMasks.PathMask))
                {
                    // The path crosses this tile

                    //Debug.Log(tilePosition);
                    tilePosition.z = 0f;

                    _invalidTiles.Add(tilePosition);
                }
                
            }
        }
    }

    public void ClickOnGrid()
    {
        //Debug.Log("Clicked on grid!");

        // Hide the range of the previous selected tower
        _selectedTower?.HideRangeVisual();

        CalculateSelectedTile();
        //Debug.Log($"Selected tile: {_selectedTilePosition}");

        _selectedTower = GetTowerFromTile(_selectedTilePosition);

        if (_selectedTower != null)
        {
            // Disable the tower panels
            TextUIManager.Instance.HideTowerPanels();

            // Enable the tower info panel
            TextUIManager.Instance.ShowTowerInfo();

            //Debug.Log(tower.name);
            TextUIManager.Instance.UpdateTowerInfo(_selectedTower);

            // Show the tower's range
            _selectedTower.ShowRangeVisual();
        }
        else
        {
            // Disable the tower info panel
            TextUIManager.Instance.HideTowerInfo();

            // Enable the tower panels
            TextUIManager.Instance.ShowTowerPanels();
        }
    }

    public void CalculateSelectedTile()
    {
        // Turn the mouse screen position into a world space point
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0f;

        // Get the position of the tile
        Vector3Int tilePosition = _grid.WorldToCell(worldPosition);

        // Set the selected tile position
        _selectedTilePosition = tilePosition;

        // Check if this tile is valid
        bool isValidTile = IsValidTile(_selectedTilePosition);

        // Set the selected tile indicator
        _selectedTileIndicator.transform.position = _selectedTilePosition;
        if (isValidTile) _selectedTileIndicator.color = new Color(1f, 1f, 1f, 0.4f); // Transparent white
        else _selectedTileIndicator.color = new Color(1f, 0f, 0f, 0.4f); // Transparent red
    }

    public bool IsValidTile(Vector3 tile) => !_invalidTiles.Contains(tile);

    public void AddTowerToTile(Vector3 tile, Tower tower)
    {
        _towerTiles.Add(tile, tower);

        // Set the tile as invalid
        _invalidTiles.Add(tile);
    }
    public Tower GetTowerFromTile(Vector3 tile)
    {
        _towerTiles.TryGetValue(tile, out Tower tower);
        return tower;
    }
    public void UpdateSelectedTowerTargetMode()
    {
        _selectedTower.ChangeTargetMode();

        // Update the UI again
        TextUIManager.Instance.UpdateTowerInfo(_selectedTower);
    }

    public void HighlightGridVisual(bool value) => _gridVisual.enabled = value;
    public void HighlightTileSelector(bool value) => _selectedTileIndicator.enabled = value;

    // Creates a grid of tiles
    // private void GenerateGrid()
    // {
    //     // Initialize the tiles dictionary
    //     tiles = new Dictionary<Vector2, Tile>();

    //     // Loop over every tile position
    //     for (int x = 0; x < width; x++)
    //     {
    //         for (int y = 0; y < height; y++)
    //         {
    //             // Spawn a tile object
    //             Tile spawnedTile = Instantiate(tilePrefab, new Vector3(x, y, 0f), Quaternion.identity);

    //             // Set the tiles as children of the grid manager so the hierarchy doesn't get cluttered up
    //             spawnedTile.name = $"Tile ({x}, {y})";
    //             spawnedTile.transform.SetParent(this.transform);

    //             // Save the tile in the tiles dictionary
    //             tiles[new Vector2(x, y)] = spawnedTile;
    //         }
    //     }

    //     // Move the camera to be centered on the grid (keep the z axis position the same)
    //     mainCamera.transform.position = new Vector3((width / 2f) - 0.5f,
    //                                                 (height / 2f) - 0.5f,
    //                                                 mainCamera.transform.position.z);
    // }

    // Gets a specific tile
    // public Tile GetTileAtPosition(Vector2 position)
    // {
    //     if (tiles.TryGetValue(position, out Tile tile)) return tile;

    //     return null;
    // }
    
}