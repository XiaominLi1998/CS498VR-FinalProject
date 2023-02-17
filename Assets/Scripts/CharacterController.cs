// Adapted from UnityChanControlScriptWithRgidBody.cs
// Credit:
// Mecanimのアニメーションデータが、原点で移動しない場合の Rigidbody付きコントローラ
// サンプル
// 2014/03/13 N.Kobyasahi
//
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    //Camera
    public float maxCameraPitch = 30f;
    public float minCameraPitch = -70f;

    //Animation
    public float animSpeed = 1f;
    public float lookSmoother = 3.0f;	// smoothing setting for camera motion
    public bool useCurves = true;
    public float useCurvesHeight = 0.5f;

    // Move speed & Player
    public float forwardSpeed = 3f;
    public float backwardSpeed = -1.2f;
    public float sideWaySpeed = 2.5f;
    public float jumpPower = 1.0f; 

    public float maxDistFromCenter = 50f;

    // Mouse sensitivity
    public float mouseSensitivityHorizontal = 10.0f;
    public float mouseSensitivityVertical = 10.0f;

    private GameController gameController;
    private UnityChan.RandomWind randomWind;
    private GameObject camera;

    private CapsuleCollider col;
    private Rigidbody rb;
    private Vector3 velocity;
    private float orgColHight;
    private Vector3 orgVectColCenter;
    private Animator anim;
    private AnimatorStateInfo currentBaseState;
    
    static int idleState = Animator.StringToHash ("Base Layer.Idle");
    static int locoState = Animator.StringToHash ("Base Layer.Locomotion");
    static int jumpState = Animator.StringToHash ("Base Layer.Jump");
    static int restState = Animator.StringToHash ("Base Layer.Rest");

    void Start ()
    {
        randomWind = GetComponent<UnityChan.RandomWind>();
        gameController = this.transform.parent.GetComponent<GameController>();
        camera = GameObject.Find("TPSCamera");
        anim = GetComponent<Animator> ();
        col = GetComponent<CapsuleCollider> ();
        rb = GetComponent<Rigidbody> ();
        orgColHight = col.height;
        orgVectColCenter = col.center;
    }

    void FixedUpdate ()
    {
        if(! gameController.isGameActive()) return;
        float h = Input.GetAxis ("Horizontal");
        float v = Input.GetAxis ("Vertical") ;
        randomWind.isWindActive = (h==0f && v==0f);
        v += (float)Math.Abs(h)*0.11f;
        anim.SetFloat ("Speed", v);		
        anim.SetFloat ("Direction", h); 
        h *= sideWaySpeed;	
        v *= ((v >= 0) ? forwardSpeed : -backwardSpeed);
        
        anim.speed = animSpeed;		
        currentBaseState = anim.GetCurrentAnimatorStateInfo (0);	
        rb.useGravity = true;
        velocity = new Vector3 (h, 0, v);	
        velocity = transform.TransformDirection(velocity);
    
        if (Input.GetButtonDown ("Jump")) {	
            //if (currentBaseState.nameHash == locoState) {
            if (!anim.IsInTransition(0)) {
                rb.AddForce (Vector3.up * jumpPower, ForceMode.VelocityChange);
                anim.SetBool ("Jump", true);
            }
            //}
        }
        // clamp position w.r.t. maxDistFromCenter
        Vector3 targetPosition = transform.localPosition + velocity * Time.fixedDeltaTime;
        float posX = targetPosition.x;
        float posZ = targetPosition.z;
        float norm = (float)Math.Sqrt(posX*posX+posZ*posZ);
        if(norm > maxDistFromCenter)
        {
            posX *= maxDistFromCenter/norm;
            posZ *= maxDistFromCenter/norm;
        }
        transform.localPosition = new Vector3(posX, transform.localPosition.y, posZ);
        
        //Camera
        float mouseRotateH = Input.GetAxis("Mouse X");
        float mouseRotateV = Input.GetAxis("Mouse Y");
        float originalCameraPitch = camera.transform.localEulerAngles.x;
        float targetCameraPitch = originalCameraPitch- mouseRotateV * mouseSensitivityVertical;
        while(targetCameraPitch>180) targetCameraPitch-=360f;
        targetCameraPitch = Mathf.Clamp(targetCameraPitch, minCameraPitch, maxCameraPitch);
        camera.transform.RotateAround(transform.position, transform.right, targetCameraPitch-originalCameraPitch);
        camera.transform.localRotation = Quaternion.Euler(targetCameraPitch,0f, 0f);
        transform.Rotate (0, mouseRotateH * mouseSensitivityHorizontal, 0);	
        
        // animation
        if (currentBaseState.nameHash == locoState) {
            if (useCurves) {
                resetCollider ();
            }
        } else if (currentBaseState.nameHash == jumpState) { 
            if (!anim.IsInTransition (0)) {
                if (useCurves) {
                    float jumpHeight = anim.GetFloat ("JumpHeight");
                    float gravityControl = anim.GetFloat ("GravityControl"); 
                    if (gravityControl > 0) rb.useGravity = false;	
                    Ray ray = new Ray (transform.position + Vector3.up, -Vector3.up);
                    RaycastHit hitInfo = new RaycastHit ();
                    if (Physics.Raycast (ray, out hitInfo)) {
                        if (hitInfo.distance > useCurvesHeight) {
                            col.height = orgColHight - jumpHeight;	
                            float adjCenterY = orgVectColCenter.y + jumpHeight;
                            col.center = new Vector3 (0, adjCenterY, 0);	
                        } else {
                            resetCollider ();
                        }
                    }
                }
                anim.SetBool ("Jump", false);
            }
        } else if (currentBaseState.nameHash == idleState) {
            if (useCurves) {
                resetCollider ();
            }
        } else if (currentBaseState.nameHash == restState) {
            if (!anim.IsInTransition (0)) {
                anim.SetBool ("Rest", false);
            }
        }
    }

    void resetCollider ()
    {
        col.height = orgColHight;
        col.center = orgVectColCenter;
    }
}
