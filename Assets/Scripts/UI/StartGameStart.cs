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
        GameManager.Instance.SetSingleplayer(true);
        StartGame();
    }

    public void StartLocalMultiplayer()
    {
        GameManager.Instance.SetSingleplayer(false);
        StartGame();
    }

    public void LoadTestTrack(Scene scene, LoadSceneMode mode)
    {
        GameManager.Instance.SetGameState(GameState.WaitingToStart);
        SceneManager.sceneLoaded -= LoadTestTrack;
    }
}
