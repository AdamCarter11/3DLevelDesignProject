using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [Header("Refrences")]
    PlayerMovementTake2 pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrapplDistance;
    public float grappleDelay;
    public float overShootYAxis;
    public float raycastWidth;


    public Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grappleCdTimer;

    private bool grappling = false;

    private void Start() {
        pm = GetComponent<PlayerMovementTake2>();

    }

    private void Update() {
        if(Input.GetMouseButtonDown(0) && !grappling){
            //if we want grapple, uncomment this and fix
            StartGrapple();
        }
        else if(Input.GetMouseButtonDown(0) && grappling){
            StopGrapple();
        }
        if(grappleCdTimer > 0){
            grappleCdTimer -= Time.deltaTime;
        }
    }

    private void StartGrapple(){
        if(grappleCdTimer > 0){
            return;
        }
        pm.freeze = true;
        grappling = true;
        RaycastHit hit;
        if(Physics.SphereCast(cam.position, raycastWidth, cam.forward, out hit, maxGrapplDistance, whatIsGrappleable)){
            print("Start grapple");
            grapplePoint = hit.point;
            Invoke(nameof(ExcecuteGrapple), grappleDelay);
        }
        else{
            print("Stopped grappl");
            grapplePoint = cam.position + cam.forward * maxGrapplDistance;
            Invoke(nameof(StopGrapple), grappleDelay);
        }
        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }
    private void LateUpdate() {
        if(grappling){
            lr.SetPosition(0, gunTip.position);
        }
    }
    private void ExcecuteGrapple(){
        pm.freeze = false;
        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        
        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overShootYAxis;
        
        if(grapplePointRelativeYPos < 0){
            highestPointOnArc = overShootYAxis;
        }
        pm.JumpToPosition(grapplePoint, highestPointOnArc);
        Invoke(nameof(StopGrapple), 1f);
    }
    public void StopGrapple(){
        pm.freeze = false;
        grappling = false;
        grappleCdTimer = grapplingCd;
        lr.enabled = false;
    }

}
