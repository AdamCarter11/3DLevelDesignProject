using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeFloors : MonoBehaviour
{
    [SerializeField] float disappearTime;
    [SerializeField] float fadeSpeed;
    Color startColor;

    private void Start() {
        startColor = GetComponent<Renderer>().material.color;
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Player")){
            StartCoroutine(FadeOut());
            print("Trigger Disappear");
        }
    }
    IEnumerator FadeOut(){
        while(this.GetComponent<Renderer>().material.color.a >= .5f){
            Color startingColor = this.GetComponent<Renderer>().material.color;
            float fadeAmount = startingColor.a - (fadeSpeed * Time.deltaTime);

            startingColor = new Color(startingColor.r, startingColor.g, startingColor.b, fadeAmount);
            GetComponent<Renderer>().material.color = startingColor;
            if(this.GetComponent<Renderer>().material.color.a <= .5f){
                StartCoroutine(Disappear());
            }
            yield return null;
        }
        
    }
    IEnumerator Disappear(){
        GetComponent<Renderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(disappearTime);
        StartCoroutine(Appear());
    }
    IEnumerator Appear(){
        yield return new WaitForSeconds(2f);
        GetComponent<Renderer>().enabled = true;
        GetComponent<Renderer>().material.color = startColor;
        GetComponent<BoxCollider>().enabled = true;
    }
}
