using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [HideInInspector] public CharacterController charController;
    Camera gameCamera;
    public Transform playerRig;
    public Transform recoilControl;

    [Header("Mouse Control Vars")]
    public float mouseSensitivity = 1f;
    public float mouseLimitsY = 170f;
    [Header("Accuracy")]
    public float startAccuracy = 1f;
    public float Accuracy {
        get { return startAccuracy * accuracyMultiplier;  }
    }
    public float accuracyMultiplier = 1f;
    float desiredAccuracyMultiplier;

    public float idleAccuracy;
    public float jumpAccuracy;
    public float walkAccuracy;
    public float runAccuraccy;
    public float scopedAccuracyMulti;

    [Header("Control Vars")]
    public float moveSpeed = 5;
    public float runSpeed = 2f;
    public float scopedSpeedMultiplier = 0.5f;
    public float jumpForce = 20f;
    public bool isSprinting;
    public bool jump;
    public bool isGrounded;
    public float airTime;
    [Header("Hands")]
    public Hand leftHand;
    public Hand rightHand;

    [Header("Grabbing")]
    public GrabbableItem grabbedItem;
    public List<GrabbableItem> guns = new List<GrabbableItem>();

    [Header("Gun Control")]
    public GunGrabState gunState = new GunGrabState();

    public static PlayerController main;

    public Gun GrabbedGun
    { 
        get 
        { 
            if(grabbedItem is Gun)
                return grabbedItem as Gun;

            return null;
        } 
    }
    public bool IsReloading
    {
        get
        {
            return PoseAnimator.main.reloading;
        }
    }
    public bool IsSwitching
    {
        get 
        {
            return PoseAnimator.main.isSwitching;
        }
    }

    public PlayerPose CurrentPlayerPose
    {
        get
        {
            return PoseAnimator.main.playerPose;
        }
    }

    
    private void Awake()
    {
        main = this;
        charController = GetComponent<CharacterController>();
        gameCamera = Camera.main;
        
    }

    

    public void GrabItem(GrabbableItem _item)
    {
        grabbedItem = _item;

        leftHand.Grab(_item.leftHandPose);
        rightHand.Grab(_item.rightHandPose);
        
        grabbedItem.OnEquip();
    }


    private void Start()
    {
        LockMouse(true);

        foreach (var g in guns)
        {
            g.gameObject.SetActive(false);
        }
        SwitchGun();
    }

    private void Update()
    {
        Crosshair.main.TurnOn(gunState == GunGrabState.Normal && !isSprinting && !IsSwitching);
        UpdateAccuracy();
        
        if (Input.GetMouseButton(0))
        {
            if (!IsSwitching)
            {
                PullTrigger();
                isSprinting = false;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            ReleaseTrigger();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (grabbedItem is Gun)
            {
                (grabbedItem as Gun).SwitchShootingMode();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (!IsSwitching)
            {
                if (isSprinting)
                    isSprinting = false;

                ChangeScopeState();
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!IsSwitching)
            {
                if (isSprinting) 
                    isSprinting = false;
                Reload();
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (Time.timeScale == 1)
                Time.timeScale = 0.1f;
            else
                Time.timeScale = 1f;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (charController.isGrounded)
            {
                upForce = Vector3.zero;
                jump = true;
            }

        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            PoseAnimator.main.SwitchGunAnimation();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            
            if (isGrounded)
            {
                if (!Input.GetMouseButton(0))
                {
                    if (!IsReloading)
                    {
                        isSprinting = !isSprinting;


                        if (isSprinting)
                        {
                            if (gunState == GunGrabState.Scoped)
                            {
                                ChangeScopeState();
                            }
                        }
                    }
                }
            }
        }
        

        HandleMovevement();
        HandleMouse();
    }
    
    public void UpdateAccuracy()
    {
        if (CurrentPlayerPose == PlayerPose.Idle)
            desiredAccuracyMultiplier = idleAccuracy;
        if (CurrentPlayerPose == PlayerPose.Run)
            desiredAccuracyMultiplier = runAccuraccy;
        if (CurrentPlayerPose == PlayerPose.Walk)
            desiredAccuracyMultiplier = walkAccuracy;
        if (!isGrounded)
            desiredAccuracyMultiplier *= jumpAccuracy;
        if (gunState == GunGrabState.Scoped)
            desiredAccuracyMultiplier *= scopedAccuracyMulti;



        accuracyMultiplier = Mathf.Lerp(accuracyMultiplier, desiredAccuracyMultiplier, 10f * Time.deltaTime);
    }

    public void Reload()
    {
        if(Inventory.main.GetAmmoAmount(GrabbedGun.ammoType) == 0) return;

        PoseAnimator.main.Reload(GrabbedGun);
    }
    public void SwitchGun()
    {

        var i = grabbedItem == null ? 0 : guns.IndexOf(grabbedItem);

        if (i < guns.Count - 1)
            i++;
        else
            i = 0;

        if (grabbedItem == null)
            i = 0;


        if (grabbedItem != null)
            grabbedItem.gameObject.SetActive(false);

        GrabItem(guns[i]);

        AmmoUI.main.ShowAmmo(GrabbedGun);
        ShootingModeUI.main.UpdateShootingMode(GrabbedGun.shootingMode);
        grabbedItem.gameObject.SetActive(true);

    }

    public void ChangeScopeState()
    {
        if (GrabbedGun != null)
        {
            if (gunState == GunGrabState.Normal)
                gunState = GunGrabState.Scoped;
            else
                gunState = GunGrabState.Normal;

            if (gunState == GunGrabState.Normal)
            {
                WeaponProceduralAnimator.main.weaponInitLocation = GrabbedGun.usualPosition;
                WeaponProceduralAnimator.main.weaponInitOrientation = Quaternion.Euler(GrabbedGun.usualRotation);
            }
            else
            {
                WeaponProceduralAnimator.main.weaponInitLocation = GrabbedGun.scopePosition;
                WeaponProceduralAnimator.main.weaponInitOrientation = Quaternion.Euler(GrabbedGun.scopeRotation);
            }

            
        }
    }

    void PullTrigger()
    {
        if (grabbedItem is Gun)
        {
            (grabbedItem as Gun).Shoot();
        }
    }
    void ReleaseTrigger()
    {
        if (grabbedItem is Gun)
        {
            (grabbedItem as Gun).ReleaseTrigger();
        }
    }

    Vector3 moveVector;
    Vector3 upForce;
    public void HandleMovevement()
    {
        isGrounded = charController.isGrounded;
        var gravity = Time.deltaTime * Physics.gravity;
        var camForward = gameCamera.transform.forward;
        camForward.y = 0;
        camForward = camForward.normalized;

        var camRight = gameCamera.transform.right;
        camRight.y = 0;
        camRight = camRight.normalized;

        var forwardVector = Input.GetAxisRaw("Vertical") * camForward;
        var horizontalVector = Input.GetAxisRaw("Horizontal") * camRight;
        float scopedMulti = gunState == GunGrabState.Scoped ? scopedSpeedMultiplier : 1f;

        float sprintMulti = isSprinting ? runSpeed : 1f;

        if(isGrounded)
            moveVector = Vector3.Lerp(moveVector, (forwardVector + horizontalVector).normalized * Time.deltaTime * moveSpeed * scopedMulti * sprintMulti, 50f * Time.deltaTime);
        else
            moveVector = Vector3.Lerp(moveVector, (forwardVector + horizontalVector).normalized * Time.deltaTime * moveSpeed * scopedMulti * sprintMulti, 1f * Time.deltaTime);


        if (jump)
        {
            upForce += Vector3.up * jumpForce;
            jump = false;
        }

        if (!isGrounded)
            upForce += Time.deltaTime * Physics.gravity * 2f * (1f + airTime);// * (1f + airTime);
        //else
        //    jumpForce = Vector3.zero;

        if (isGrounded)
        {
            
            airTime = 0;
        }
        else
        {
            airTime += Time.deltaTime;
        }

        if ((Mathf.Abs(moveVector.x) + Mathf.Abs(moveVector.z)) > 0.001f)
        {
            if (isSprinting)
                PoseAnimator.main.SwitchPose(PlayerPose.Run);
            else
                PoseAnimator.main.SwitchPose(PlayerPose.Walk);
            
        }
        else
        {
            PoseAnimator.main.SwitchPose(PlayerPose.Idle);
            isSprinting = false;
        }

        charController.Move(moveVector + gravity + upForce * Time.deltaTime);

        var directionImpulse = Input.GetAxisRaw("Horizontal") * Vector3.right + Input.GetAxisRaw("Vertical") * Vector3.forward * scopedSpeedMultiplier;
        WeaponProceduralAnimator.main.DirectionImpulse(directionImpulse * 0.3f);
        //WeaponProceduralAnimator.main.OrientationImpulse(Quaternion.Euler( Input.GetAxisRaw("Horizontal") * Vector3.forward * 5f + Input.GetAxisRaw("Vertical") * Vector3.right * 5f));
    }

    public void HandleMouse()
    {
        var rotX = Input.GetAxis("Mouse X") * mouseSensitivity;
        var rotY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        charController.transform.Rotate(charController.transform.up, rotX);

        var newRotY = playerRig.transform.localEulerAngles.x - rotY;
        
        if (newRotY < 0)
            newRotY += 360f;

        if (newRotY < 180)
            newRotY = Mathf.Clamp(newRotY, 0f, mouseLimitsY);
        else
            newRotY = Mathf.Clamp(newRotY, 360 - mouseLimitsY, 360f);

        playerRig.transform.localEulerAngles = new Vector3(newRotY, playerRig.transform.localEulerAngles.y, playerRig.transform.localEulerAngles.z);

        var orientationImpulse = Quaternion.Euler(((-Vector3.up * rotX) + (Vector3.right * rotY)) * 33f * Time.deltaTime);

        WeaponProceduralAnimator.main.OrientationImpulse(orientationImpulse);
    }

    public void LockMouse(bool _locked)
    {
        Cursor.lockState = _locked ? CursorLockMode.Locked : CursorLockMode.None;
        
    }
}

public enum GunGrabState
{
    Normal,
    Scoped
}
