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
    public TMP_Text playerLaps;
}
