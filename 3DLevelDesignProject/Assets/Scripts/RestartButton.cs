using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour
{
    //make sure to set these variables in the inespector
    [SerializeField] AudioSource bgm;
    [SerializeField] GameObject slider;

    private void Start() {
        //Changes volume to the saved value
        bgm.volume = PlayerPrefs.GetFloat("Volume");
        //Changes UI to visualize the saved volume
        slider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("Volume");
    }
    public void RestartFunction(){
        /////  To Load a new scene (or restart in your case), use this line:
        SceneManager.LoadScene("Level1");
    }
    public void QuitGame(){
        /////  To Quit out of the program, use this line:
        Application.Quit();
    }

    //make sure this function is public and attached to an empty gameobject
    //  then that gameobject is attached to the sliders on value changed slot
    public void VolumeSlider(){
        //what actually changes the volume when the slider changes
        bgm.volume = slider.GetComponent<Slider>().value;
        //this saves the volume so when you quit and restart the volume will still be the same
        PlayerPrefs.SetFloat("Volume", slider.GetComponent<Slider>().value); 
    }
}
