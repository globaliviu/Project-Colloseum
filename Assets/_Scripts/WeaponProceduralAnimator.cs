using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProceduralAnimator : MonoBehaviour
{
    public float comeBackForce = 15f;
    public float comeToForce = 15;
    public float directionMagnitudeLimit = 0.02f;

    public Transform weaponHolster;
    public Vector3 weaponInitLocation;
    public Quaternion weaponInitOrientation;


    public Vector3 directionForce;
    public Quaternion orientationForce;

    public static WeaponProceduralAnimator main;

    private void Awake()
    {
        main = this;
        NewWeaponInit(weaponHolster.localPosition, weaponHolster.localRotation);
    }

    public void NewWeaponInit(Vector3 _loc, Quaternion _rot)
    {
        weaponInitLocation = _loc;
        weaponInitOrientation = _rot;
    }


    public void DirectionImpulse(Vector3 _direction)
    {
        directionForce += _direction * Time.deltaTime;

        if (directionForce.magnitude > directionMagnitudeLimit)
        {
            directionForce = directionForce.normalized * directionMagnitudeLimit;
        }
    }
    public void OrientationImpulse(Quaternion _direction)
    {
        orientationForce *= _direction;
    }

    public void LateUpdate()
    {
        float scopedMulti = PlayerController.main.gunState == GunGrabState.Scoped ? 1.5f : 1f;

        directionForce = Vector3.Lerp(directionForce, Vector3.zero, comeBackForce * Time.deltaTime);
        orientationForce = Quaternion.Lerp(orientationForce, Quaternion.identity, comeBackForce * Time.deltaTime);

        weaponHolster.localPosition = Vector3.Lerp(weaponHolster.localPosition, weaponInitLocation + directionForce, comeToForce * scopedMulti * Time.deltaTime );
        weaponHolster.localRotation = Quaternion.Lerp(weaponHolster.localRotation, weaponInitOrientation * orientationForce, comeToForce * scopedMulti * Time.deltaTime);
    }

}
