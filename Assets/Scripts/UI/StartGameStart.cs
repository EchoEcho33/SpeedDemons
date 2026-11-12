using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameStart : MonoBehaviour
{
    private void StartGame()
    {
        // SceneManager.sceneLoaded += LoadTestTrack;
        SceneManager.LoadSceneAsync("Scenes/Main/CharacterSelect");
    }

    public void InputSelect()
    {
        SceneManager.LoadSceneAsync("Scenes/MAIN/InputSelect");
    }

    public void StartSinglePlayer()
    {
        GameManager.Instance.numPlayers = 1;
        StartGame();
    }

    public void StartLocalMultiplayer()
    {
        // TODO: add UI to set number of players. defaults to whatever is set in gamemanager rn
        // - gianluca
        StartGame();
        print("opened charselect");
    }
    
}
