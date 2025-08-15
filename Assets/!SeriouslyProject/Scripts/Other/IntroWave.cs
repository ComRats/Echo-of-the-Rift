using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroWave : MonoBehaviour
{
    [SerializeField] private int _waitTime;

    private void Start()
    {
        StartCoroutine(WaitForVideo());
    }

    IEnumerator WaitForVideo()
    {
        yield return new WaitForSeconds(_waitTime);
        SceneManager.LoadScene(1);
    }
}
