using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField]
    private Canvas _mainRaceCanvas;
    [SerializeField]
    public Image primaryItemIcon;
    [SerializeField]
    public Image secondaryItemIcon;

    [SerializeField]
    public Image progressBar;

    //filler for example
    [SerializeField]
    public Image backgroundImage;

    [SerializeField]
    public TMP_Text playerLaps;
}
