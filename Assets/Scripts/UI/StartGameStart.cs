using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameStart : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.sceneLoaded += LoadTestTrack;
        SceneManager.LoadSceneAsync("TestTrack");
        
    }

    public void LoadTestTrack(Scene scene, LoadSceneMode mode)
    {
        GameManager.Instance.SetGameState(GameState.WaitingToStart);
        SceneManager.sceneLoaded -= LoadTestTrack;
    }
}
