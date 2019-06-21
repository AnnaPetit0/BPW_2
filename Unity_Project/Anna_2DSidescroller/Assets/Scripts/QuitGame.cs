using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuitGame : MonoBehaviour
{
    public void DoQuitGame()
    {
        Application.Quit();
        Debug.Log("QUIT");
    }
}
