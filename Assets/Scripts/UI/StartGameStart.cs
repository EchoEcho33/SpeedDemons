using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameStart : MonoBehaviour
{
    private void StartGame()
    {
        SceneManager.sceneLoaded += LoadTestTrack;
        SceneManager.LoadSceneAsync("Scenes/Main/CharacterSelect");
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
    }

    public void LoadTestTrack(Scene scene, LoadSceneMode mode)
    {
        GameManager.Instance.SetGameState(GameState.WaitingToStart);
        SceneManager.sceneLoaded -= LoadTestTrack;
    }
}
