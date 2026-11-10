using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RaceUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField]
    private Canvas _mainRaceCanvas;
    
    public Canvas MainRaceCanvas => _mainRaceCanvas;
    
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

    [SerializeField] private TextMeshProUGUI countdownText; 
    
    private int currCountdownValue;

    [SerializeField] public RectTransform viewport;

    public Camera playerCamera;

    public Event countdownFinished; 

    private void Start()
    {
        _primaryItemIcon.enabled = false;
        _secondaryItemIcon.enabled = false;
        
        viewport.gameObject.SetActive(false);
        
        // need to delay ui init so that we can get camera rect from the PlayerInputManager's splitscreen
        // this can also be delayed by a certain amount of time if needed. right now it enables after the countdown is complete
        GameManager.Instance.UIManagerObject.GetComponent<GlobalRaceUI>().CountdownFinished += OnCountdownFinished;
        
    }

    private void OnCountdownFinished()
    {
        ApplyViewport();
    }
    
    // these player specific countdown functions are unused for now
    IEnumerator TickCountdown(int countdown)
    {
        
        currCountdownValue = countdown;
        while (currCountdownValue > 0)
        {
            countdownText.text = currCountdownValue.ToString();
            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
        }

        if (currCountdownValue <= 0)
        {
            // cant set active? doing this for now
            countdownText.enabled = false;
            
        }
    }

    public void TickTimer(int countdown)
    {
        currCountdownValue = countdown;
        if (currCountdownValue > 0)
        {
            countdownText.text = currCountdownValue.ToString();
            countdownText.enabled = true;
        }
        else
        {
            countdownText.enabled = false;
        }
    }

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

    public void PickUpItem(Item item)
    {
        if (!_primaryItemIcon.IsActive())
        {
            _primaryItemIcon.sprite = item.getIcon();
            _primaryItemIcon.enabled = true;
        } else if (!_secondaryItemIcon.IsActive())
        {
            _secondaryItemIcon.sprite = item.getIcon();
            _secondaryItemIcon.enabled = true;
        }
    }

    public bool UseItem()
    {
        if (_secondaryItemIcon.IsActive())
        {
            _primaryItemIcon.sprite = _secondaryItemIcon.sprite;
            _secondaryItemIcon.enabled = false;
            return true;
        } else if (_primaryItemIcon.IsActive())
        {
            _primaryItemIcon.enabled = false;
            _secondaryItemIcon.enabled = false;
            return true;
        }
        return false;
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

    private void ApplyViewport()
    {
        if (playerCamera == null)
        {
            Debug.LogError("raceui cam assignment missing", this);
            return;
        }

        Rect cameraRect = playerCamera.rect;

        viewport.anchorMin = cameraRect.min;
        viewport.anchorMax = cameraRect.max;
        viewport.offsetMin = Vector2.zero;
        viewport.offsetMax = Vector2.zero;
        viewport.localScale = Vector3.one;
        viewport.gameObject.SetActive(true);
    }
}
