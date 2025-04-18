using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextUIManager : MonoBehaviour
{
    // Singleton
    public static TextUIManager Instance;

    // UI
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _livesText;
    [SerializeField] private TextMeshProUGUI _orbsText;
    [SerializeField] private TextMeshProUGUI _wavesText;
    [SerializeField] private GameObject _nextButton;

    [Header("Tower Panels")]
    [SerializeField] private GameObject _towerPanels;

    [Header("Tower Info")]
    [SerializeField] private GameObject _towerInfoPanel;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _rangeText;
    [SerializeField] private TextMeshProUGUI _attackPowerText;
    [SerializeField] private TextMeshProUGUI _attackSpeedText;
    [SerializeField] private TextMeshProUGUI _targetModeText;

    private void Awake()
    {
        // Make sure there is only one instance
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void ShowNextButton() => _nextButton.SetActive(true);
    public void HideNextButton() => _nextButton.SetActive(false);

    // Tower Panels
    public void ShowTowerPanels() => _towerPanels.SetActive(true);
    public void HideTowerPanels() => _towerPanels.SetActive(false);

    // Tower Info
    public void ShowTowerInfo() => _towerInfoPanel.SetActive(true);
    public void HideTowerInfo() => _towerInfoPanel.SetActive(false);
    public void UpdateTowerInfo(Tower tower)
    {
        _nameText.text = $"Name: {tower.Name}";
        _rangeText.text = $"Range: {tower.Range}";
        _attackPowerText.text = $"Attack Power: {tower.AttackPower}";
        _attackSpeedText.text = $"Attack Speed: {tower.AttackSpeed}";
        _targetModeText.text = $"{tower.TargetMode}";
    }

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
