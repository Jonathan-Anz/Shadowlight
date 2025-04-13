using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    // Singleton
    public static GridManager instance;

    private Grid _grid;
    [SerializeField] private MeshRenderer _gridVisual; // Set in inspector
    [SerializeField] private SpriteRenderer selectedTileIndicator; // Set in inspector

    //[SerializeField] private int width, height; // Set in inspector
    //[SerializeField] private Tile tilePrefab; // Set in inspector
    //[SerializeField] private Transform mainCamera; // Set in inspector

    //private Dictionary<Vector2, Tile> tiles;


    private void Awake()
    {
        // Make sure there is only one Grid Manager
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        _grid = GetComponent<Grid>();

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0f;
        Vector3Int cellPosition = _grid.WorldToCell(worldPosition);
        selectedTileIndicator.transform.position = cellPosition;
        //Debug.Log(cellPosition);
    }

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