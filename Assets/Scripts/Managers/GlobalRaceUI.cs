using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

/*
 * This is a Global UI that appears in the center of the screen, handling the countdown timer and final race timer.
 * Maybe bad practice in terms of inheritance but it works in the case of appearing for all splitscreen players. 
 */
public class GlobalRaceUI : MonoBehaviour
{
    [Header("UI Elements")] [SerializeField]
    private Canvas _mainRaceCanvas;

    public Canvas MainRaceCanvas => _mainRaceCanvas;

    [SerializeField] private TextMeshProUGUI countdownText;

    private int currCountdownValue;

    public event Action CountdownFinished;

    public void Start()
    {

    }

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
            CountdownFinished?.Invoke();
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

}
