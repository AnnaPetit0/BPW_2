using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class background_music : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
}
