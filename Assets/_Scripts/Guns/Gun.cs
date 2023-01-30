using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : GrabbableItem
{
    [Header("Parameters")]
    public float fireRate;
    public int magSize;
    public int curMagSize;

    public List<ShootingModes> availableShootingModes = new List<ShootingModes>();
    public ShootingModes shootingMode = new ShootingModes();
    public AmmoType ammoType;
    public Transform Muzzle;

    public LayerMask shootingLayer;
    public RecoilController recoilController;

    public GameObject muzzleFX;
    public GameObject decalFX;
    [Header("Animation parameters")]
    public Vector3 usualPosition;
    public Vector3 scopePosition;
    public Vector3 usualRotation;
    public Vector3 scopeRotation;

    public Vector3 runPosOffset;
    public Vector3 runRotOffset;
    public Transform animatedPart;


    float lastTimeShot;

    public GameObject magazinePrefab;
    public GameObject curMagazine;


    public bool canShoot = true;


    public void ReleaseTrigger()
    {
        if(shootingMode == ShootingModes.Single)
            canShoot = true;
    }

    public void Shoot()
    {
        if (PoseAnimator.main.reloading) return;


        if (curMagSize == 0)
        {
            //reload
            PlayerController.main.Reload();
            return;
        }

        if (shootingMode == ShootingModes.Single && !canShoot) return;
        if (Time.time - lastTimeShot < fireRate - Time.deltaTime) return;

        lastTimeShot = Time.time;
        //bullet
        
        
        
        var accuracy = PlayerController.main.Accuracy;

        var dir = Muzzle.forward + Muzzle.up * Random.Range(-accuracy, accuracy) * 0.02f + Muzzle.right * Random.Range(-accuracy, accuracy) * 0.02f;

        BallisticsManager.main.NewProjectile(Muzzle.position, dir, 200f, 0.05f);

        /*
        Ray ray = new Ray(Muzzle.position, Muzzle.forward + Muzzle.up * Random.Range(-accuracy, accuracy) * 0.02f + Muzzle.right * Random.Range(-accuracy, accuracy) * 0.02f);


        if (Physics.Raycast(ray, out RaycastHit hit, 1000, shootingLayer))
        {
            var fx = Instantiate(decalFX, hit.point, Quaternion.identity);
            fx.transform.LookAt(hit.normal + hit.point);
        }

        var muzzleFx = Instantiate(muzzleFX, Muzzle);
        muzzleFx.transform.localPosition = Vector3.zero;
        muzzleFx.transform.localRotation = Quaternion.identity;
        muzzleFx.transform.localScale = Vector3.one * Random.Range(0.8f, 1.2f);
        muzzleFx.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360f));

        Destroy(muzzleFx, 0.1f);
        */
        //apply recoil
        recoilController.ApplyRecoil();
        animatedPart.localPosition = new Vector3(0f, 0f, -0.15f);

        curMagSize--;
        //Inventory.main.ConsumeAmmo(ammoType);
        AmmoUI.main.ShowAmmo(this);

        if (shootingMode == ShootingModes.Single)
        {
            canShoot = false;
        }

    }

    void LateUpdate()
    {
        Ray ray = new Ray(Muzzle.position, Muzzle.forward);
        var pos = Muzzle.position + Muzzle.forward * 1000f;
        if (Physics.Raycast(ray, out RaycastHit hit, 1000, shootingLayer))
        {
            pos = hit.point;
        }

        //lr.positionCount = 2;
        //lr.SetPosition(0, Muzzle.position);
        //lr.SetPosition(1, pos);
        if(!PoseAnimator.main.reloading)
            animatedPart.localPosition = Vector3.Lerp(animatedPart.localPosition, Vector3.zero, 20f * Time.deltaTime);

        Crosshair.main.SetPosition(pos);
    }

    public void SwitchShootingMode()
    {
        if (availableShootingModes.Count <= 1) return;

        int curMode = availableShootingModes.IndexOf(shootingMode);

        curMode++;

        if (curMode > availableShootingModes.Count - 1)
            curMode = 0;

        shootingMode = availableShootingModes[curMode];

        ShootingModeUI.main.UpdateShootingMode(shootingMode);
    }

}
