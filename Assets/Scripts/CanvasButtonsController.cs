using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasButtonsController : MonoBehaviour
{
    public void RestartGame()
    {
        //get index of current scene
        var sceneIndex = SceneManager.GetActiveScene().buildIndex;
        //reload currens scene
        SceneManager.LoadScene(sceneIndex);
    }

    public void OpenGoogle()
    {
        Application.OpenURL("https://www.google.com");
    }
}
