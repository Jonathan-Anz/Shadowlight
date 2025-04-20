using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    // Getters
    public Vector3 SelectedTilePosition => _selectedTilePosition;


    private void Awake()
    {
        // Make sure there is only one instance
        if (Instance != null) Destroy(gameObject);
        else Instance = this;

        _grid = GetComponent<Grid>();
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
        _selectedTower?.ToggleRangeVisual(false);

        CalculateSelectedTile();
        //Debug.Log($"Selected tile: {_selectedTilePosition}");

        _selectedTower = GetTowerFromTile(_selectedTilePosition);

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
    public void RemoveTowerFromTile(Tower tower)
    {
        // Remove from towers list
        if (!_towerTiles.Remove(_selectedTilePosition))
        {
            Debug.LogWarning($"Could not find a tower at {_selectedTilePosition}");
        }

        // Set the tile as valid again
        _invalidTiles.Remove(_selectedTilePosition);

        // Destroy the tower
        Destroy(tower.gameObject);
    }
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
        RemoveTowerFromTile(_selectedTower);

        // Disable the tower info panel
        TextUIManager.Instance.ToggleTowerInfo(false);

        // Enable the tower panels
        TextUIManager.Instance.ToggleTowerPanels(true);

        _selectedTower = null;
    }

    public void ClearGrid()
    {
        // Delete all the towers from the game and dictionary
        Tower[] towers = _towerTiles.Values.ToArray();
        _towerTiles.Clear();

        foreach (Tower tower in towers)
        {
            Destroy(tower.gameObject);
        }

        // Mark all the tiles as valid
        _invalidTiles.Clear();
    }

    public void HighlightGridVisual(bool value) => _gridVisual.enabled = value;
    public void HighlightTileSelector(bool value) => _selectedTileIndicator.enabled = value;

}