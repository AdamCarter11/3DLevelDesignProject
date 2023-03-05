using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [Header("Refrences")]
    PlayerMovement pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrapplDistance;
    public float grappleDelay;
    public float overShootYAxis;


    public Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grappleCdTimer;

    private bool grappling;

    private void Start() {
        pm = GetComponent<PlayerMovement>();

    }

    private void Update() {
        if(Input.GetMouseButtonDown(0)){
            //if we want grapple, uncomment this and fix
            //StartGrapple();
        }
        if(grappleCdTimer > 0){
            grappleCdTimer -= Time.deltaTime;
        }
    }

    private void StartGrapple(){
        pm.freeze = true;
        if(grappleCdTimer > 0){
            return;
        }
        grappling = true;
        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrapplDistance, whatIsGrappleable)){
            grapplePoint = hit.point;
            Invoke(nameof(ExcecuteGrapple), grappleDelay);
        }
        else{
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
