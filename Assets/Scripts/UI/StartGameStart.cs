using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameStart : MonoBehaviour
{
    private void StartGame()
    {
        SceneManager.sceneLoaded += LoadTestTrack;
        GameManager.Instance.SetGameState(GameState.Lobby);
        GameManager.Instance.LoadScene("Scenes/Main/CharacterSelect");
    }

    public void InputSelect()
    {
        GameManager.Instance.LoadScene("Scenes/MAIN/InputSelect");
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
        
        // TODO: need to test, but this is what should handle # of local players
        GameManager.Instance.numPlayers = GameManager.Instance.JoinedInputDevices.Count;
        print("opened charselect");
    }
    
    public void LoadTestTrack(Scene scene, LoadSceneMode mode)
    {
        
        SceneManager.sceneLoaded -= LoadTestTrack;
    }
}
