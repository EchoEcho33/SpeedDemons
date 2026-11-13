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
        
        SceneManager.sceneLoaded += OnRaceSceneLoaded;
        SceneManager.LoadScene("Scenes/MAIN/MainRace");
    }

    // changed the name so its not test track anymore
    private void OnRaceSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainRace")
        {
            return;
        }
        // protecting the racemaanger init since its takes longer now
        SceneManager.sceneLoaded -= OnRaceSceneLoaded;
        GameManager.Instance.SetGameState(GameState.WaitingToStart);
    }
}
