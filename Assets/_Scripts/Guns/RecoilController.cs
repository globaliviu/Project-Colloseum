using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilController : MonoBehaviour
{
    [Header("Recoil Settings")]
    public float recoilForce = 1f;
    public float upForce = 5f;
    public float backwardForce = 35f;
    public float orientationUpImpulse = 7.5f;

    public AnimationCurve recoilCurve;
    public float comingBackForce;
    public float comingToForce;

    Transform recoilTransform;
    Gun gun;
    public float recoilTimer;
    Quaternion desiredRotation = Quaternion.identity;
    private void Awake()
    {
        gun = GetComponent<Gun>();
        gun.onEquip += OnEquip;
        gun.recoilController = this;
    }

    private void Update()
    {
        if (recoilTransform == null) return;

        recoilTimer -= Time.deltaTime;

        if (recoilTimer < 0)
            recoilTimer = 0;
        if (recoilTimer > 1f)
            recoilTimer = 1f;

        Crosshair.main.ApplyScale((recoilTimer * recoilForce * 0.5f + 1f));

        comingBackForce = recoilCurve.Evaluate(1f - recoilTimer) * comingToForce;

        recoilTransform.localRotation = Quaternion.Lerp(recoilTransform.localRotation, desiredRotation, comingToForce * Time.deltaTime);
        desiredRotation = Quaternion.Lerp(desiredRotation, Quaternion.identity, comingBackForce * Time.deltaTime);
    }

    public void OnEquip()
    {
        recoilTransform = PlayerController.main.recoilControl;
    }
    
    public void ApplyRecoil()
    {
        float scopedMulti = PlayerController.main.gunState == GunGrabState.Scoped ? 0.5f : 1f;

        desiredRotation *= Quaternion.Euler(-Vector3.right * Random.Range(recoilForce / 2f, recoilForce) * scopedMulti);
        desiredRotation *= Quaternion.Euler(Vector3.up * Random.Range(-recoilForce / 2f, recoilForce / 2f) * scopedMulti);

        recoilTimer += gun.fireRate * 2f;

        WeaponProceduralAnimator.main.DirectionImpulse((Vector3.up * upForce * scopedMulti) - Vector3.forward * backwardForce * scopedMulti * scopedMulti);
        WeaponProceduralAnimator.main.OrientationImpulse(Quaternion.Euler(-Vector3.right * orientationUpImpulse * scopedMulti));
        WeaponProceduralAnimator.main.OrientationImpulse(Quaternion.Euler(Vector3.up * Random.Range(-orientationUpImpulse, orientationUpImpulse) * 0.5f * scopedMulti));
    }

    private void OnDestroy()
    {
        gun.onEquip -= OnEquip;
    }
}
