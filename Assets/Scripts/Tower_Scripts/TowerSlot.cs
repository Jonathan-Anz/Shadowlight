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
    [Header("Tower Details")]
    [SerializeField] private TowerData _tower;
    [SerializeField] private int _orbCost;

    [Header("Misc")]
    [SerializeField] private Sprite _lockImage;
    // Temporary, will need to get this somewhere
    //[SerializeField] private float _tileSize = 1;

    //private TowerSlotManager _towerSlotManager;

    // References
    private Tower _towerToPlace;
    private Image _slotImage;

    private TextMeshProUGUI _orbCostText;
    private bool _isActive;
    //private Vector3 _towerPosition;
    //private Vector3 _startPos;

    // Getters
    public TowerData Tower => _tower;


    private void Awake()
    {
        // Get references to components
        _slotImage = gameObject.transform.GetChild(0).GetComponent<Image>();
        _orbCostText = GetComponentInChildren<TextMeshProUGUI>();

        // Set sprites and text
        InitializeTowerSlot();
    }

    private void InitializeTowerSlot()
    {
        //_towerSlotManager = GetComponentInParent<TowerSlotManager

        // Makes sure the sprite is active
        _slotImage.gameObject.SetActive(true);

        // "Locks" the slot if there is no prefab
        if (_tower == null || _tower.towerPrefab == null)
        {
            _orbCostText.gameObject.SetActive(false);
            _isActive = false;

            // Set the sprite to a lock
            _slotImage.sprite = _lockImage;
        }
        else
        {
            _orbCostText.gameObject.SetActive(true);
            _isActive = true;

            // Set the orb cost text
            _orbCostText.text = $"{_orbCost} orbs";

            // Set sprite
            _slotImage.sprite = _tower.towerSprite;
        }
    }

    // Set the tower for the slot
    // Called by the tower manager
    public void SetTower(TowerData tower, int cost)
    {
        _tower = tower;
        _orbCost = cost;

        InitializeTowerSlot();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Check if the player has enough orbs or if the slot is active
        if (GameManager.Instance.PlayerOrbs < _orbCost || !_isActive) return;

        // Stores position to restore it after dragging.
        //_startPos = transform.position;
        //_startPos = _towerImage.transform.position;

        GridManager.Instance.CalculateSelectedTile();

        // Use the grid tile position instead
        //_towerPosition = GridManager.Instance.SelectedTilePosition;

        // Instantiate the tower object
        _towerToPlace = Instantiate(_tower.towerPrefab, GridManager.Instance.SelectedTile.Position, Quaternion.identity).GetComponent<Tower>();
        _towerToPlace.InitializeTower(_orbCost);
        _towerToPlace.DisableTower(false);
        _towerToPlace.ToggleRangeVisual(true);

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

        // Make sure the slot is active
        if (!_isActive) return;

        // Check if the player has enough orbs
        if (GameManager.Instance.PlayerOrbs < _orbCost) return;

        // Calculate the current selected tile
        GridManager.Instance.CalculateSelectedTile(); 
        _towerToPlace.transform.position = GridManager.Instance.SelectedTile.Position;

        // Set the position to the current tile
        //transform.position = Camera.main.WorldToScreenPoint(GridManager.Instance.SelectedTilePosition);
        //_towerImage.transform.position = Camera.main.WorldToScreenPoint(GridManager.Instance.SelectedTilePosition);

        // TODO: Make the object snap to the tiles as well.
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Finds the mouse position relative to the world position.
        // Without using camera, it will use canvas position instead.
        //Vector3 towerPosition = _towerSlotManager.cam.ScreenToWorldPoint(transform.position);

        // Make sure slot is active
        if (!_isActive) return;

        // Check if the player has enough orbs
        if (GameManager.Instance.PlayerOrbs < _orbCost) return;

        // Use the grid tile position instead
        //_towerPosition = GridManager.Instance.SelectedTilePosition;

        // Snaps tower position to tile.
        //towerPosition = SnapToTileSize(towerPosition, _tileSize);

        // Resets tower slot sprite.
        //transform.position = _startPos;
        //_towerImage.transform.position = _startPos;

        // TODO: unhide the UI?

        // Unhighlight the range visual
        _towerToPlace.ToggleRangeVisual(false);

        // Un-highlight the grid
        GridManager.Instance.HighlightGridVisual(false);
        GridManager.Instance.HighlightTileSelector(false);

        // Check if the tile is valid
        if (!GridManager.Instance.SelectedTile.IsValidTile())
        {
            Destroy(_towerToPlace.gameObject);
            return;
        }

        // Creates the tower in that position.
        //Tower tower = Instantiate(_towerPrefab, towerPosition, Quaternion.identity).GetComponent<Tower>();

        // Save the tower to the selected tile
        //GridManager.Instance.AddTowerToSelectedTile(_towerToPlace);
        GridManager.Instance.SelectedTile.AddTowerToTile(_towerToPlace);

        // Initialize the tower
        //_towerToPlace.InitializeTower();

        // Re-enable attacking
        _towerToPlace.DisableTower(true);

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
