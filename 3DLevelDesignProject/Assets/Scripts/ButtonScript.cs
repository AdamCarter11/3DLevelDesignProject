using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public void restartGame(){
        SceneManager.LoadScene("Room1And2");
    }
    public void quitGame(){
        Application.Quit();
    }
}
