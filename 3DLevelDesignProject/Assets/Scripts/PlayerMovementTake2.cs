using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovementTake2 : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    [SerializeField] int maxJumps;
    int resetJumps;
    float resetJumpForce;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    bool activeGrapple;
    [HideInInspector] public bool freeze;
    [SerializeField] float disappearTime;
    [SerializeField] float fadeSpeed;
    Color startColor;
    GameObject currFloor;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        resetJumps = maxJumps;
        resetJumpForce = jumpForce;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);
        if(grounded){
            jumpForce = resetJumpForce;
            maxJumps = resetJumps;
        }

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
            maxJumps--;
        }
        else if(Input.GetKey(jumpKey) && readyToJump && maxJumps > 0){
            jumpForce /= 2;
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
            maxJumps--;
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }

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

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("dFloor")){
            currFloor = other.gameObject;
            startColor = currFloor.GetComponent<Renderer>().material.color;
            StartCoroutine(FadeOut());
            
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

    //disappearing platforms
    IEnumerator Disappear(){
        print("Trigger Disappear");
        currFloor.transform.position = new Vector3(currFloor.transform.position.x, currFloor.transform.position.y - 1f, currFloor.transform.position.z);
        currFloor.SetActive(false);
        yield return new WaitForSeconds(disappearTime);
        StartCoroutine(Appear());
    }
    IEnumerator Appear(){
        yield return new WaitForSeconds(1f);
        print("activate");
        currFloor.SetActive(true);
        currFloor.transform.position = new Vector3(currFloor.transform.position.x, currFloor.transform.position.y + 1f, currFloor.transform.position.z);
        currFloor.GetComponent<Renderer>().material.color = new Color(startColor.r, startColor.g, startColor.b, 1);
    }
}