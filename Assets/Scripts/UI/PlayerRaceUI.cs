using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRaceUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField]
    private Image _primaryItemIcon;
    [SerializeField]
    private Image _secondaryItemIcon;

    [SerializeField]
    private TMP_Text _playerLaps;

    [SerializeField]
    private Image _progressBar;

    //filler for example
    [SerializeField]
    private Image _backgroundImage;

    public PlayerRaceUI(Image primaryItemIcon, Image secondaryItemIcon, TMP_Text playerLaps, Image progressBar, Image backgroundImage)
    {
        _primaryItemIcon = primaryItemIcon;
        _secondaryItemIcon = secondaryItemIcon;
        _playerLaps = playerLaps;
        _progressBar = progressBar;
        _backgroundImage = backgroundImage;

        _playerLaps.text = "1 / " + GameManager.Instance.race.maxLaps;
    }

    public void UpdateLapUI(int currLap)
    {
        _playerLaps.text = currLap + " / " + GameManager.Instance.race.maxLaps;
    }

    public void UpdateBar(int abilityFill, int maxVal)
    {
        _progressBar.fillAmount = abilityFill / (float)maxVal;

        if (abilityFill == maxVal) { _backgroundImage.color = new Color(0, 255, 0); }
            else { _backgroundImage.color = new Color(255, 255, 255); }
    }
}
