using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

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
    [SerializeField] private GameObject _pauseMenu;

    [Header("Tower Panels")]
    [SerializeField] private GameObject _towerPanels;

    [Header("Tower Info")]
    [SerializeField] private GameObject _towerInfoPanel;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _rangeText;
    [SerializeField] private TextMeshProUGUI _attackPowerText;
    [SerializeField] private TextMeshProUGUI _attackSpeedText;
    [SerializeField] private TextMeshProUGUI _targetModeText;
    [SerializeField] private TextMeshProUGUI _sellValueText;

    private void Awake()
    {
        // Make sure there is only one instance
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }

    // UI
    public void ToggleNextButton(bool value) => _nextButton.SetActive(value);
    public void TogglePauseMenu(bool value) => _pauseMenu.SetActive(value);

    // Tower Panels
    public void ToggleTowerPanels(bool value) => _towerPanels.SetActive(value);

    // Tower Info
    public void ToggleTowerInfo(bool value) => _towerInfoPanel.SetActive(value);
    public void UpdateTowerInfo(Tower tower)
    {
        _nameText.text = $"Name: {tower.Name}";
        _rangeText.text = $"Range: {tower.Range}";
        _attackPowerText.text = $"Attack Power: {tower.AttackPower}";
        _attackSpeedText.text = $"Attack Speed: {tower.AttackSpeed}";
        _targetModeText.text = $"{tower.TargetMode}";
        _sellValueText.text = $"+{tower.SellValue} orbs";
    }

    public void UpdateWavesText(int currentWave, int maxWaves)
    {
        if (currentWave > maxWaves)
        {
            _wavesText.text = $"<b>Level Complete!<b>";
        }
        else
        {
            _wavesText.text = $"<b>Wave</b>: {currentWave} / {maxWaves}";
        }   
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
