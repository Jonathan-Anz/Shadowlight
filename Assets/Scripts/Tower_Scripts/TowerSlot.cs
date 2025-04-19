using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Tower to create
    [SerializeField] private GameObject _towerPrefab;
    [SerializeField] private GameObject _towerImage;
    [SerializeField] private TextMeshProUGUI _orbCostText;
    [SerializeField] private int _orbCost;

    // Temporary, will need to get this somewhere
    //[SerializeField] private float _tileSize = 1;

    private TowerSlotManager _towerSlotManager;
    private Vector3 _startPos;


    private void Awake()
    {
        // Set the orb cost text
        _orbCostText.text = _orbCost.ToString();

        // Not needed?
        //InitializeTowerSlot();
    }

    private void InitializeTowerSlot()
    {
        _towerSlotManager = GetComponentInParent<TowerSlotManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Check if the player has enough orbs
        if (GameManager.Instance.PlayerOrbs < _orbCost) return;

        // Stores position to restore it after dragging.
        //_startPos = transform.position;
        _startPos = _towerImage.transform.position;

        // TODO: hide the UI?

        // Highlight the grid
        GridManager.Instance.HighlightGridVisual(true);
        GridManager.Instance.HighlightTileSelector(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Old code
        // Makes the object follow the mouse.
        //transform.position = Input.mousePosition;

        // Check if the player has enough orbs
        if (GameManager.Instance.PlayerOrbs < _orbCost) return;

        // Calculate the current selected tile
        GridManager.Instance.CalculateSelectedTile();

        // Set the position to the current tile
        //transform.position = Camera.main.WorldToScreenPoint(GridManager.Instance.SelectedTilePosition);
        _towerImage.transform.position = Camera.main.WorldToScreenPoint(GridManager.Instance.SelectedTilePosition);

        // TODO: Make the object snap to the tiles as well.
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Finds the mouse position relative to the world position.
        // Without using camera, it will use canvas position instead.
        //Vector3 towerPosition = _towerSlotManager.cam.ScreenToWorldPoint(transform.position);

        // Check if the player has enough orbs
        if (GameManager.Instance.PlayerOrbs < _orbCost) return;

        // Use the grid tile position instead
        Vector3 towerPosition = GridManager.Instance.SelectedTilePosition;

        // Snaps tower position to tile.
        //towerPosition = SnapToTileSize(towerPosition, _tileSize);

        // Resets tower slot sprite.
        //transform.position = _startPos;
        _towerImage.transform.position = _startPos;

        // TODO: unhide the UI?

        // Un-highlight the grid
        GridManager.Instance.HighlightGridVisual(false);
        GridManager.Instance.HighlightTileSelector(false);

        // Check if the tile is valid
        if (!GridManager.Instance.IsValidTile(towerPosition)) return;

        // Creates the tower in that position.
        //Tower tower = Instantiate(_towerPrefab, towerPosition, Quaternion.identity).GetComponent<Tower>();
        Tower tower = Instantiate(_towerPrefab, towerPosition, Quaternion.identity).GetComponent<Tower>();

        // Save the tower to the grid dictionary
        GridManager.Instance.AddTowerToTile(towerPosition, tower);

        // Initialize the tower
        tower.InitializeTower();

        // Remove the orbs from the player
        GameManager.Instance.AddOrbs(-_orbCost);
    }

    // Snaps position to middle of tiles.
    private Vector3 SnapToTileSize(Vector3 position, float tileSize)
    {
        Vector3 newPosition = Vector3.zero;
        newPosition.x = SnapToFloat(position.x, tileSize);
        newPosition.y = SnapToFloat(position.y, tileSize);

        return newPosition;
    }

    // Snaps float to middle of the target float multiple.
    private float SnapToFloat(float num, float floatSize)
    {
        int multiple = (int) Mathf.Floor(num / floatSize);

        // Middle of the float
        float remainder = floatSize / 2;

        return floatSize * multiple + remainder;
    }

}
