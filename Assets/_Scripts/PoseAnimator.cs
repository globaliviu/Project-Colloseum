using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseAnimator : MonoBehaviour
{
    public float comeBackForce = 15f;
    public float comeToForce = 15;

    public Transform poseTransform;

    public Vector3 desiredPos;
    public Quaternion desiredRot;

    public PlayerPose playerPose = new PlayerPose();
    [Header("Idle Pose")]
    public float idleFrequency;
    public float idleYoffset;

    [Header("Walk Pose")]
    public float walkFrequency;
    public float walkYoffset;
    public float walkRotYOffset;
    public float walkRotFrequency;
    public float walkXoffset;
    public float walkXFrequency;

    public static PoseAnimator main;

    
    Hand leftHand;
    Hand rightHand;
    private void Awake()
    {
        main = this;
    }
    private void Start()
    {
        leftHand = PlayerController.main.leftHand;
        rightHand = PlayerController.main.rightHand;
    }
    public void Reload(Gun gun)
    {
        if(!reloading)
            StartCoroutine(IReload(gun));
    }
    public bool reloading = false;
    public IEnumerator IReload(Gun gun)
    {
        reloading = true;
        comeToForce = 20f;
        var magPos = gun.curMagazine.transform.localPosition;
        var magRot = gun.curMagazine.transform.localRotation;
        var magParent = gun.curMagazine.transform.parent;

        desiredRot = Quaternion.Euler(-10f, 0f, 0);

        yield return new WaitForSeconds(0.1f);
        desiredRot = Quaternion.Euler(2f, 0f, 0);
        yield return new WaitForSeconds(0.05f);
        StartCoroutine(IMagFall(gun.curMagazine));

        var newMag = Instantiate(gun.magazinePrefab).GetComponentInChildren<HandPoseReference>();
        newMag.transform.parent.parent = magParent;
        newMag.transform.parent.localRotation = magRot;
        newMag.transform.parent.localPosition = magPos -  new Vector3(0.1f, 0.15f, 0);
        
        float d1 = 0.2f;
        leftHand.Grab(newMag, d1);
        yield return new WaitForSeconds(d1);

        desiredRot = Quaternion.Euler(-30f, 0f, -20f);

        var p = newMag.transform.parent.localPosition;
        var r = newMag.transform.parent.localRotation;
        float t = 0;
        float d = 0.2f;
        while (t < d)
        {
            newMag.transform.parent.localPosition = Vector3.Lerp(p, magPos, t / d);
            newMag.transform.parent.localRotation = Quaternion.Lerp(r, magRot, t / d);
            t += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
            leftHand.Grab(newMag);
        }

        desiredRot = Quaternion.Euler(-34f, 0f, -20f);
        desiredPos = Vector3.up * 0.01f;
        yield return new WaitForSeconds(Time.deltaTime * 4f);

        newMag.transform.parent.localPosition = magPos;
        newMag.transform.parent.localRotation = magRot;

        gun.curMagazine = newMag.transform.parent.gameObject;

        //hand to animated object
        leftHand.Grab(gun.animatedPart.GetComponentInChildren<HandPoseReference>(), 0.3f);
        yield return new WaitForSeconds(0.3f);
        desiredRot = Quaternion.Euler(-34f, 0f, 15f);
        float t2 = 0;
        float d2 = 0.3f;
        var p2 = gun.animatedPart.localPosition;
        while (t2 < d2)
        {
            if (t2 < d2 / 2f)
                gun.animatedPart.localPosition = Vector3.Lerp(p2, new Vector3(0f, 0f, -0.15f), t2 / (d2 / 2f));
            else
                gun.animatedPart.localPosition = Vector3.Lerp(new Vector3(0f, 0f, -0.15f), new Vector3(0f, 0f, 0f), (t2 - d2 / 2f) / (d2 / 2f));
            t2 += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
            
        }

        leftHand.Grab(gun.leftHandPose, 0.15f);
        yield return new WaitForSeconds(0.15f);
        reloading = false;
        comeToForce = 15f;
    }

    public IEnumerator IMagFall(GameObject _mag)
    {
        _mag.transform.parent = null;//.SetActive(false);

        float t = 0;
        float d = 1f;

        while (t < d)
        {
            _mag.transform.position += Vector3.down * 9.7f * Time.deltaTime ;
            yield return null;
            t += Time.deltaTime;
        }

        yield return new WaitForSeconds(Time.deltaTime);

        Destroy(_mag);
    }


    public void Update()
    {
        if (!reloading)
        {
            float scopedMulti = PlayerController.main.gunState == GunGrabState.Scoped ? 0.5f : 1f;

            if (playerPose == PlayerPose.Idle)
            {
                desiredPos = new Vector3(0, Mathf.Sin(Time.time * idleFrequency) * idleYoffset * scopedMulti, 0);
                desiredRot = Quaternion.identity;
            }
            if (playerPose == PlayerPose.Walk)
            {
                desiredPos = new Vector3(Mathf.Sin(Time.time * walkXFrequency) * walkXoffset * scopedMulti, Mathf.Sin(Time.time * walkFrequency) * walkYoffset * scopedMulti, 0);
                desiredRot = Quaternion.Euler(0, Mathf.Sin(Time.time * walkRotFrequency * scopedMulti) * walkRotYOffset * scopedMulti, 0);

            }
            if (playerPose == PlayerPose.Run)
            {
                desiredPos = new Vector3(Mathf.Sin(Time.time * walkFrequency) * walkYoffset * scopedMulti, Mathf.Sin(Time.time * walkFrequency) * walkYoffset * scopedMulti, 0);
                desiredRot = Quaternion.Euler(0, -70 + Mathf.Sin(Time.time * walkRotFrequency * scopedMulti) * walkRotYOffset * scopedMulti, 0);
            }
        }
        //directionForce = Vector3.Lerp(directionForce, Vector3.zero, comeBackForce * Time.deltaTime);
        //orientationForce = Quaternion.Lerp(orientationForce, Quaternion.identity, comeBackForce * Time.deltaTime);

        poseTransform.localPosition = Vector3.Lerp(poseTransform.localPosition, desiredPos, comeToForce * Time.deltaTime);
        poseTransform.localRotation = Quaternion.Lerp( poseTransform.localRotation, desiredRot, comeToForce * Time.deltaTime);
    }
}

public enum PlayerPose
{
    Idle,
    Walk,
    Run
}
