using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSlot : MonoBehaviour, IDragHandler, IEndDragHandler
{
    // Tower to create
    [SerializeField] private Object _towerPrefab;

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

        // Locks the tower position to a tile.
        towerPosition.x = Mathf.Round(towerPosition.x);
        towerPosition.y = Mathf.Round(towerPosition.y);

        // Makes sure that the object position is in z = 0.
        towerPosition.z = 0;

        // Creates the tower in that position.
        Object tower = Instantiate(_towerPrefab, towerPosition, Quaternion.identity);

        // Resets tower slot sprite.
        transform.position = _startPos;
    }

 
}
