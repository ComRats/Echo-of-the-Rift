using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneLoader
{
    public void LoadToScene(string sceneToLoad, GameObject objectToLoad, Vector3 positionToLoad)
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
