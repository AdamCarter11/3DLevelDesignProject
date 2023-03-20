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
    [SerializeField] int maxJumps;
    int resetJumps;
    bool canJump = true;
    float resetJumpForce;
    [HideInInspector] public bool wallRunning;
    bool canMove = true;

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

    //grappling hook vars
    public bool freeze;
    public bool activeGrapple;

    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        startingSpeed = moveSpeed;
        resetJumps = maxJumps;
        resetJumpForce = jumpForce;
    }
    
    void Update()
    {
        //checks if we are grounded
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * .5f + .2f, whatIsGround);
        if(wallRunning){
            grounded = true;
        }
        //float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        //rotY += mouseX;

        if(freeze){
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
        }
        else{
            GetInputs();
        }
        
        SpeedControl();

        //applies drag if we are grounded (prevents ice-like feel)
        if(grounded){
            jumpForce = resetJumpForce;
            maxJumps = resetJumps;
            if(!activeGrapple){
                rb.drag = groundDrag;
            }
        }
        else{
            rb.drag = 0;
        }

        //respawn player if they fall off the map
        if(transform.position.y < -15){
            transform.position = new Vector3(0f, 1.5f, 0f);
        }

        
    }
    private void GetInputs(){
        if(activeGrapple){
            return;
        }
        //get input
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
        if(Input.GetKey(KeyCode.Space) && canJump && grounded){
            //print("Jumped");
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
            maxJumps--;
        }
        else if(Input.GetKey(KeyCode.Space) && canJump && maxJumps > 0){
            jumpForce /= 2;
            canJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
            maxJumps--;
        }

        if(Input.GetKey(KeyCode.LeftShift)){
            canMove = false;
        }
        else{
            canMove = true;
        }
    }

    private void MovePlayer(){
        //calculate force and apply to player
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
        //rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);

        if(canMove){
            if(grounded){
                rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);
                //freeze = false;
            }
            else if(!grounded){
                rb.AddForce(moveDir.normalized * moveSpeed * 10f * airMult, ForceMode.Force);
            }
        }
        else{
            rb.velocity = Vector3.zero;
        }

        //sets player speed if they are wall running
        if(wallRunning){
            moveSpeed = wallRunSpeed;
            canJump = true;
        }
        else{
            moveSpeed = startingSpeed;
        }
    }
    private void FixedUpdate() {
        if(!activeGrapple){
            MovePlayer();
        }
    }

    //caps player speed
    private void SpeedControl(){
        if(activeGrapple){
            return;
        }
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
            //currFloor = other.gameObject;
            //startColor = currFloor.GetComponent<Renderer>().material.color;
            //StartCoroutine(FadeOut());
            
        }
        if(enableMovementOnNextTouch){
            enableMovementOnNextTouch = false;
            ResetRestrictions();
            GetComponent<GrapplingHook>().StopGrapple();
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

    //grappling functions
    private Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajHeight){
        float grav = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * grav * trajHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajHeight / grav) + Mathf.Sqrt(2*(displacementY - trajHeight)/grav));
        return velocityXZ + velocityY;
    }

    private Vector3 velocityToSet;
    private void SetVelocity(){
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
    }
    public void ResetRestrictions(){
        activeGrapple = false;
    }
    private bool enableMovementOnNextTouch;
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight){
        activeGrapple = true;
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), .1f);
        Invoke(nameof(ResetRestrictions), 3f);
    }

    //disappearing platforms
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
        currFloor.GetComponent<Renderer>().material.color = new Color(startColor.r, startColor.g, startColor.b, 1);
    }
}
