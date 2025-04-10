using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextUIManager : MonoBehaviour
{
    // UI
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _livesText;
    [SerializeField] private TextMeshProUGUI _orbsText;
    [SerializeField] private TextMeshProUGUI _wavesText;

    public void UpdateWavesText(int currentWave, int maxWaves)
    {
        _wavesText.text = $"<b>Wave</b>: {currentWave} / {maxWaves}";
    }

    public void UpdateLivesText(int playerLives)
    {
        _livesText.text = $"<b>Lives</b>: {playerLives}";
    }

    public void UpdateOrbsText(int playerOrbs)
    {
        _orbsText.text = $"<b>Orbs</b>: {playerOrbs}";
    }
}
