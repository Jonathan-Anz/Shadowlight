using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// This script is needed to use OnMouseDown() on the grid visual object,
// which is a child object of the grid manager
public class GridClicker : MonoBehaviour
{
    private void OnMouseDown()
    {
        // Make sure the player can't click through the menu
        if (EventSystem.current.IsPointerOverGameObject()) return;

        GridManager.Instance.ClickOnGrid();
    }
}