using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject highlightSprite; // Set in inspector


    // OnMouse functions need a BoxCollider2D on the tile to work
    private void OnMouseEnter()
    {
        // Enable the highlight sprite
        highlightSprite.SetActive(true);
    }
    private void OnMouseExit()
    {
        // Disable the highlight sprite
        highlightSprite.SetActive(false);
    }
    private void OnMouseDown()
    {
        //Debug.Log(this.transform.position);
    }
}