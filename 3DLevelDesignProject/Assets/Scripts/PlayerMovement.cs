using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //This script controls the player movement

    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] Transform orientation;
    [SerializeField] float groundDrag;
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCooldown;
    [SerializeField] float airMult;
    bool canJump = true;

    [Header("Ground Check")]
    [SerializeField] float playerHeight;
    [SerializeField] LayerMask whatIsGround;
    bool grounded;

    float horizontalInput;
    float verticalInput;
    Vector3 moveDir;
    Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
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
}
