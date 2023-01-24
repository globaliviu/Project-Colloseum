using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : GrabbableItem
{
    [Header("Parameters")]
    public float fireRate;
    public int magSize;
    public int curMagSize;
    
    public Transform Muzzle;

    public LayerMask shootingLayer;
    public RecoilController recoilController;

    public GameObject muzzleFX;
    public GameObject decalFX;
    public Vector3 usualPosition;
    public Vector3 scopePosition;
    public Transform animatedPart;


    float lastTimeShot;

    public GameObject magazinePrefab;
    public GameObject curMagazine;

    public void Shoot()
    {
        if (PoseAnimator.main.reloading) return;
        if (Time.time - lastTimeShot < fireRate - Time.deltaTime) return;


        lastTimeShot = Time.time;
        //bullet
        Ray ray = new Ray(Muzzle.position, Muzzle.forward);
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

        //applu recoil
        recoilController.ApplyRecoil();
        animatedPart.localPosition = new Vector3(0f, 0f, -0.15f);

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

    

}
