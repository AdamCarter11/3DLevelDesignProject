using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thirdPersonCam : MonoBehaviour
{
    //This script is what controls the third person camera

    [Header("References")]
    [SerializeField] Transform orientation;
    [SerializeField] Transform player;
    [SerializeField] Transform playerObj;
    [SerializeField] Rigidbody rb;
    //change this var in the inspector to increase/decrease rotation speed
    [SerializeField] float rotationSpeed;

    private void Start() {
        //locks & hides the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //gets the direction you want to view based on the players rotation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(inputDir != Vector3.zero){
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
    }
}
