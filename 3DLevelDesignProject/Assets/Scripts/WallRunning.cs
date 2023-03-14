using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    //This script is used for wall running and wall holding
    
    [Header("Wall Running")]
    [SerializeField] LayerMask whatIsWall;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float wallRunForce, wallClimbSpeed;
    [SerializeField] float maxWallRunTime;
    float wallRunTimer;

    [Header("Input")]
    float horizontalInput;
    float verticalInput;

    [Header("Detection")]
    [SerializeField] float wallCheckDist;
    [SerializeField] float minJumpHeight;
    RaycastHit leftWallHit, rightWallHit;
    bool wallLeft, wallRight;

    [Header("References")]
    [SerializeField] Transform orientation;
    PlayerMovement pm;
    Rigidbody rb;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update() {
        CheckForWall();
        StateMachine();
    }
    private void FixedUpdate() {
        if(pm.wallRunning){
            WallRunMovement();
        }
    }

    private void CheckForWall(){
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDist, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDist, whatIsWall);
    }

    private bool AboveGround(){
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine(){
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if((wallLeft || wallRight) && verticalInput > 0 && AboveGround()){
            //start wallRun
            if(!pm.wallRunning){
                StartWallRun();
            }
        }
        else{
            if(pm.wallRunning){
                StopWallRun();
            }
        }
    }

    private void StartWallRun(){
        pm.wallRunning = true;
    }
    private void WallRunMovement(){
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude){
            wallForward = -wallForward;
        }

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        if (Input.GetKeyDown(KeyCode.LeftShift))
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);

        //push player towards wall
        if(!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0)){
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }
        
    }
    private void StopWallRun(){
        pm.wallRunning = false;
        rb.useGravity = true;
    }
}
