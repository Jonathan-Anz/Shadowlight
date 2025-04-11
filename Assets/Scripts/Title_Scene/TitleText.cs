using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleText : MonoBehaviour
{
    private TextMeshProUGUI _titleText;
    void Start()
    {
        // Get title text
        _titleText = GetComponent<TextMeshProUGUI>();

        // Change title text to have color
        _titleText.text = $"Shadow<color=#E5BE8F>Light</color>";
    }
}
