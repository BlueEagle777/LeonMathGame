using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public string sceneToLoad; // Specify the scene to load in the Inspector

    public void SwitchScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
