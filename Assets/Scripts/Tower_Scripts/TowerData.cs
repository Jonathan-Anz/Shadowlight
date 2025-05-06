using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Tower", menuName = "ScriptableObjects/TowerData", order = 1)]
public class TowerData : ScriptableObject
{
    public TowerType towerType;
    public GameObject towerPrefab;
    public Sprite towerSprite;
    public int towerCost;

    // May contain upgrade information in the future
}
