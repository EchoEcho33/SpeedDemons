using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartRace : MonoBehaviour
{
    [SerializeField]
    CharSelect UIController;

    public void begin()
    {
        // this variable is currently unused.
        // When multiplayer and other characters are implemented, convert racerSelection to a list and pass it into gamecontroller
        RacerSelection racer = UIController.racerSelection;
        
        if (UIController.IsSingleplayer())
            GameManager.Instance.SelectCharactersAndKarts(racer);
        
        SceneManager.LoadScene("Scenes/MAIN/MainRace");
    }
}
