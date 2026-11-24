using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LoadingSceenDelay(0.5f));
    }

    // IT'S TOO FAST
    private IEnumerator LoadingSceenDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadSceneAsync(GameManager.Instance.ScenePath);
    }
}
