using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingPlatform : MonoBehaviour
{
    [SerializeField] float range;
    [SerializeField] float speed;
    [SerializeField] bool vertical;
    [SerializeField] bool forwardBack;
    Vector3 min, max;

    private void Start() {
        min = transform.position;
        max = transform.position;
    }
    private void Update() {
        
    }
    private void FixedUpdate() {
        PlatMovement();
    }

    private void PlatMovement(){
        if(vertical){
            transform.position = new Vector3(transform.position.x, Mathf.PingPong(Time.time * speed, max.y + range - min.y) + min.y, transform.position.z);
        }
        else if(forwardBack){
            transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.PingPong(Time.time * speed, max.z + range - min.z) + min.z);
        }
        else{
            transform.position = new Vector3(Mathf.PingPong(Time.time * speed, max.x + range - min.x) + min.x, transform.position.y, transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            other.gameObject.transform.parent.parent = this.transform;
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            other.transform.parent.parent = null;
        }
    }
}
