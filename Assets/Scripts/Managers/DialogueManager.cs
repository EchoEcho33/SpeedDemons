using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance {get; private set;}
    
    // Buffer of time between when voicelines can be called
    [SerializeField]
    private float _voicelineCooldown = 10.0f;
    [SerializeField]
    private GameObject _dialogueBoxContainer;
    [SerializeField]
    private Image _portrait;
    [SerializeField]
    private TextMeshPro _dialogueText;
    private bool _onCooldown = false;

    public enum VoiceLineType
    {
        PASSING,
        GETTINGPASSED,
        HITTING,
        GETTINGHIT,
        ULT,
        WINNING
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void TriggerVoiceline(Character character, VoiceLineType voiceLineType)
    {
        if (!_onCooldown)
        {
            Character.CharacterVoices voices =  character.GetVoiceLines();
            string text = "";

            switch(voiceLineType)
            {
                case VoiceLineType.PASSING:
                    text = voices.passing.text;
                    break;

                case VoiceLineType.GETTINGPASSED:
                    text = voices.gettingPassed.text;
                    break;
                
                case VoiceLineType.HITTING:
                    text = voices.hitting.text;
                    break;
                
                case VoiceLineType.GETTINGHIT:
                    text = voices.gettingHit.text;
                    break;

                case VoiceLineType.ULT:
                    text = voices.ult.text;
                    break;

                case VoiceLineType.WINNING:
                    text = voices.winning.text;
                    break;
            }

            _dialogueText.text = text;
            _portrait.sprite = character.PolaroidIcon;
            _dialogueBoxContainer.SetActive(true);

            // Play voiceline logic and HIDE dialogue box a few seconds after the voiceline is over
            // For Sean to implement hehe

            _dialogueBoxContainer.SetActive(false);

            StartCoroutine(VoiceLineCooldown());
        }
    }

    private IEnumerator VoiceLineCooldown()
    {
        _onCooldown = true;
        yield return new WaitForSeconds(_voicelineCooldown);
        _onCooldown = false;
    }
}
