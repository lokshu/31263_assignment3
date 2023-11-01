using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneNameToLoad; // The name of the scene you want to load.

    private void OnMouseDown()
    {
        SceneManager.LoadScene(sceneNameToLoad);
    }
}
