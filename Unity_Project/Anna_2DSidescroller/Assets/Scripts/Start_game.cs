using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Start_game : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Debug.Log("Start Game");
    }
}