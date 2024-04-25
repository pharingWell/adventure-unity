using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        // Start the coroutine to load the new scene
        StartCoroutine(LoadNewSceneAfterDelay("House", 2.5f));
    }

    IEnumerator LoadNewSceneAfterDelay(string sceneName, float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Load the new scene
        SceneManager.LoadScene(sceneName);
    }
}