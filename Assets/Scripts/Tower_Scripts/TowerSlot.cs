using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Tower to create
    [SerializeField] private Object _towerPrefab;

    // Temporary, will need to get this somewhere
    //[SerializeField] private float _tileSize = 1;

    private TowerSlotManager _towerSlotManager;
    private Vector3 _startPos;

    // Start is called before the first frame update
    void Start()
    {
        InitializeTowerSlot();
    }

    private void InitializeTowerSlot()
    {
        _towerSlotManager = GetComponentInParent<TowerSlotManager>();
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Stores position to restore it after dragging.
        _startPos = transform.position;

        // TODO: hide the tower selection panel

        // Highlight the grid
        GridManager.Instance.HighlightGridVisual(true);
        GridManager.Instance.HighlightTileSelector(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Old code
        // Makes the object follow the mouse.
        //transform.position = Input.mousePosition;

        // Calculate the current selected tile
        GridManager.Instance.CalculateSelectedTile();

        // Set the position to the current tile
        transform.position = Camera.main.WorldToScreenPoint(GridManager.Instance.SelectedTilePosition);

        // TODO: Make the object snap to the tiles as well.
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Finds the mouse position relative to the world position.
        // Without using camera, it will use canvas position instead.
        //Vector3 towerPosition = _towerSlotManager.cam.ScreenToWorldPoint(transform.position);

        // Use the grid tile position instead
        Vector3 towerPosition = GridManager.Instance.SelectedTilePosition;

        // Snaps tower position to tile.
        //towerPosition = SnapToTileSize(towerPosition, _tileSize);

        // Creates the tower in that position.
        Object tower = Instantiate(_towerPrefab, towerPosition, Quaternion.identity);

        // Resets tower slot sprite.
        transform.position = _startPos;

        // TODO: unhide the tower selection panel

        // Un-highlight the grid
        GridManager.Instance.HighlightGridVisual(false);
        GridManager.Instance.HighlightTileSelector(false);
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
