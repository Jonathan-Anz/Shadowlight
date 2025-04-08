using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSlot : MonoBehaviour, IDragHandler, IEndDragHandler
{
    // Tower to create
    [SerializeField] private Object _towerPrefab;

    // Temporary, will need to get this somewhere
    [SerializeField] private float _tileSize = 1;

    private TowerSlotManager _towerSlotManager;
    private Vector3 _startPos;

    // Start is called before the first frame update
    void Start()
    {
        // Starting position bugs if there is no waiting time for some reason...
        Invoke("InitializeTowerSlot", 0.01f);
    }

    private void InitializeTowerSlot()
    {
        _towerSlotManager = GetComponentInParent<TowerSlotManager>();
        _startPos = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Finds the mouse position relative to the world position.
        // Without using camera, it will use canvas position instead.
        Vector3 towerPosition = _towerSlotManager.camera.ScreenToWorldPoint(transform.position);

        // Snaps tower position to tile.
        towerPosition = SnapToTileSize(towerPosition, _tileSize);

        // Creates the tower in that position.
        Object tower = Instantiate(_towerPrefab, towerPosition, Quaternion.identity);

        // Resets tower slot sprite.
        transform.position = _startPos;
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
