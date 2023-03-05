using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //This script controls the player movement

    [Header("Movement")]
    [SerializeField] float moveSpeed;
    private float startingSpeed;
    [SerializeField] Transform orientation;
    [SerializeField] float groundDrag;
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCooldown;
    [SerializeField] float airMult;
    [SerializeField] float wallRunSpeed;
    bool canJump = true;
    [HideInInspector] public bool wallRunning;

    [Header("Ground Check")]
    [SerializeField] float playerHeight;
    [SerializeField] LayerMask whatIsGround;
    bool grounded;

    float horizontalInput;
    float verticalInput;
    Vector3 moveDir;
    Rigidbody rb;

    [Header("Platform vars")]
    [SerializeField] float disappearTime;
    [SerializeField] float fadeSpeed;
    Color startColor;
    GameObject currFloor;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        startingSpeed = moveSpeed;
    }
    
    void Update()
    {
        //checks if we are grounded
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * .5f + .2f, whatIsGround);

        MovePlayer();
        SpeedControl();

        //applies drag if we are grounded (prevents ice-like feel)
        if(grounded){
            rb.drag = groundDrag;
        }
        else{
            rb.drag = 0;
        }

        //respawn player if they fall off the map
        if(transform.position.y < -15){
            transform.position = new Vector3(0f, 1.5f, 0f);
        }
    }

    private void MovePlayer(){
        //get input
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
        if(Input.GetKey(KeyCode.Space) && canJump && grounded){
            print("Jumped");
            canJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //calculate force and apply to player
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);

        if(grounded){
            rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if(!grounded){
            rb.AddForce(moveDir.normalized * moveSpeed * 10f * airMult, ForceMode.Force);
        }

        //sets player speed if they are wall running
        if(wallRunning){
            moveSpeed = wallRunSpeed;
        }
        else{
            moveSpeed = startingSpeed;
        }
    }

    //caps player speed
    private void SpeedControl(){
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > moveSpeed){
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump(){
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump(){
        canJump = true;
    }

    private void OnCollisionEnter(Collision other) {
         if(other.gameObject.CompareTag("dFloor")){
            currFloor = other.gameObject;
            startColor = currFloor.GetComponent<Renderer>().material.color;
            StartCoroutine(FadeOut());
            
        }
    }

    IEnumerator FadeOut(){
        while(currFloor.GetComponent<Renderer>().material.color.a >= .5f){
            Color startingColor = currFloor.GetComponent<Renderer>().material.color;
            float fadeAmount = startingColor.a - (fadeSpeed * Time.deltaTime);

            startingColor = new Color(startingColor.r, startingColor.g, startingColor.b, fadeAmount);
            currFloor.GetComponent<Renderer>().material.color = startingColor;
            if(currFloor.GetComponent<Renderer>().material.color.a <= .5f){
                StartCoroutine(Disappear());
            }
            yield return null;
        }
        
    }
    IEnumerator Disappear(){
        print("Trigger Disappear");
        currFloor.transform.position = new Vector3(currFloor.transform.position.x, currFloor.transform.position.y - 1f, currFloor.transform.position.z);
        currFloor.SetActive(false);
        yield return new WaitForSeconds(disappearTime);
        StartCoroutine(Appear());
    }
    IEnumerator Appear(){
        yield return new WaitForSeconds(2f);
        print("activate");
        currFloor.SetActive(true);
        currFloor.transform.position = new Vector3(currFloor.transform.position.x, currFloor.transform.position.y + 1f, currFloor.transform.position.z);
        currFloor.GetComponent<Renderer>().material.color = startColor;
    }
}
