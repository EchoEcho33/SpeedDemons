using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField]
    private Canvas _mainRaceCanvas;
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

    public void UpdateLap(int currLap)
    {
        if (_playerLaps != null)
        {
            _playerLaps.text = currLap + " / " + RaceManager.Instance.maxLaps;
        } else
        {
            Debug.LogError("PlayerLaps text not set in UIManager.");
        }
    }

    public void PickUpItem(string itemId)
    {
        
    }

    public void UseItem()
    {
        
    }

    public void UpdateBar(int abilityBar, int abilityMaxValue)
    {
        if (_progressBar != null)
        {
            _progressBar.fillAmount = abilityBar / (float)abilityMaxValue;
        } else
        {
            Debug.LogError("Progress Bar not set in UIManager.");
        }

        //glow green if max

        if (_backgroundImage != null)
        {
            if (abilityBar >= abilityMaxValue) { _backgroundImage.color = new Color(0, 255, 0); }
            else { _backgroundImage.color = new Color(255, 255, 255); }
        } else
        {
            Debug.LogError("Background Image not set in UIManager.");
        }
    }
}
