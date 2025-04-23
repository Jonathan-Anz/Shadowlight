using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSlotManager : MonoBehaviour
{
    // Singleton
    public static TowerSlotManager Instance;

    [Header("Tower Data")]
    [SerializeField] private List<TowerData> _towerDatas;

    [Header("References")]
    [SerializeField] private GameObject _towerSlotContainer;

    // Contains all tower slots
    private List<TowerSlot> _towerSlots = new List<TowerSlot>();

    // Number of towers in slots the player currently has
    private int towerInSlotCount;

    // Initializiation
    private void Awake()
    {
        // Make sure there is only one instance
        if (Instance != null) Destroy(gameObject);
        else Instance = this;

        // Get tower slots
        TowerSlot[] towerSlots = _towerSlotContainer.GetComponentsInChildren<TowerSlot>();
        Debug.Log(towerSlots);
        foreach (TowerSlot slot in towerSlots)
            _towerSlots.Add(slot);
    }

    // Adds tower to the leftmost empty slot
    public void AddTower(TowerType tower, int cost)
    {
        // Makes sure method does not run when tower slots are full
        if (towerInSlotCount > _towerSlots.Count)
            return;

        TowerData towerData = null;

        // Goes through the Tower Data list to find the tower needed
        foreach(TowerData data in _towerDatas)
        {
            if (data.towerType == tower)
            {
                towerData = data;
                break;
            }
        }

        // Find leftmost empty slot
        for (int i = 0; i < _towerSlots.Count; i++)
        {
            if (_towerSlots[i].Tower == null)
            {
                _towerSlots[i].SetTower(towerData, cost);
                return;
            }
        }
    }

    // Removes the tower in the given slot
    public void RemoveTowerFromSlot(int slot)
    {
        // Makes sure method does not run if tower slot is empty
        if (_towerSlots[slot].Tower == null)
            return;

        _towerSlots[slot].SetTower(null, 0);
    }


}
