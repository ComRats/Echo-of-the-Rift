using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UploadTarget : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        Player player = FindObjectOfType<Player>();
        if (player != null)
            gameObject.transform.SetParent(player.transform);
    }
}
