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

    [Header("Control Vars")]
    public float moveSpeed = 5;
    public float runSpeed = 10;
    public float scopedSpeedMultiplier = 0.5f;

    [Header("Hands")]
    public Hand leftHand;
    public Hand rightHand;

    [Header("Grabbing")]
    public GrabbableItem grabbedItem;

    public Gun GrabbedGun
    { 
        get 
        { 
            if(grabbedItem is Gun)
                return grabbedItem as Gun;

            return null;
        } 
    }

    [Header("Gun Control")]
    public GunGrabState gunState = new GunGrabState();

    public static PlayerController main;
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
        GrabItem(grabbedItem);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Shoot();
        }
        if (Input.GetMouseButtonDown(1))
        {
            ChangeScopeState();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (Time.timeScale == 1)
                Time.timeScale = 0.1f;
            else
                Time.timeScale = 1f;
        }

        HandleMovevement();
        HandleMouse();

       // GrabItem(grabbedItem);
    }

    void Reload()
    {
        PoseAnimator.main.Reload(GrabbedGun);
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
                WeaponProceduralAnimator.main.weaponInitLocation = GrabbedGun.usualPosition;
            else
                WeaponProceduralAnimator.main.weaponInitLocation = GrabbedGun.scopePosition;

            Crosshair.main.TurnOn(gunState == GunGrabState.Normal);
        }
    }

    void Shoot()
    {
        if (grabbedItem is Gun)
        {
            (grabbedItem as Gun).Shoot();
        }
    }

    public void HandleMovevement()
    {
        var gravity = Time.deltaTime * Physics.gravity;
        var forwardVector = Input.GetAxisRaw("Vertical") * gameCamera.transform.forward;
        var horizontalVector = Input.GetAxisRaw("Horizontal") * gameCamera.transform.right;
        float scopedMulti = gunState == GunGrabState.Scoped ? scopedSpeedMultiplier : 1f;
        var moveVector = gravity + (forwardVector + horizontalVector).normalized * Time.deltaTime * moveSpeed * scopedMulti;


        if ((forwardVector + horizontalVector).magnitude < 0.01f)
            PoseAnimator.main.playerPose = PlayerPose.Idle;
        else
            PoseAnimator.main.playerPose = PlayerPose.Walk;

        charController.Move(moveVector);

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
