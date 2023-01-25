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

    [Header("Run Pose")]
    public float runFrequency;
    public float runYoffset;
    public float runRotYOffset;
    public float runRotFrequency;
    public float runXoffset;
    public float runXFrequency;
    public HandPoseReference leftHandRunRef;

    public static PoseAnimator main;

    [Header("Configs")]
    public List<PlayerPoseConfig> configs = new List<PlayerPoseConfig>();
    
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
        newMag.transform.localScale = Vector3.one;
        newMag.transform.parent.localRotation = magRot;
        newMag.transform.parent.localPosition = magPos -  new Vector3(0, 0.35f, 0);
        
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

    Vector3 leftHandDesiredPos;
    public void Update()
    {
        if (!reloading)
        {
            float xAir = Mathf.Clamp( 0.06f * PlayerController.main.airTime, 0, 0.14f);
            float scopedMulti = PlayerController.main.gunState == GunGrabState.Scoped ? 0.5f : 1f;

            if (playerPose == PlayerPose.Idle)
            {
                desiredPos = new Vector3(0, Mathf.Sin(Time.time * idleFrequency) * idleYoffset * scopedMulti + xAir * scopedMulti, 0);
                desiredRot = Quaternion.identity;
            }
            if (playerPose == PlayerPose.Walk)
            {
                desiredPos = new Vector3(Mathf.Sin(Time.time * walkXFrequency) * walkXoffset * scopedMulti, Mathf.Sin(Time.time * walkFrequency) * walkYoffset * scopedMulti + xAir * scopedMulti, 0);
                desiredRot = Quaternion.Euler(0, Mathf.Sin(Time.time * walkRotFrequency * scopedMulti) * walkRotYOffset * scopedMulti, 0);

            }
            if (playerPose == PlayerPose.Run)
            {
                /*
                if (leftHand.handTarget.parent != leftHandRunRef.transform.parent)
                {
                    leftHand.Grab(leftHandRunRef, 0.3f);
                    leftHandDesiredPos = leftHandRunRef.transform.localPosition;
                }
                //new 1 hand
                leftHand.handTarget.parent = PlayerController.main.playerRig;
                leftHandDesiredPos = new Vector3(leftHandRunRef.transform.localPosition.x, leftHandRunRef.transform.localPosition.y + Mathf.Sin(Time.time * runFrequency / 2f) * 0.1f + xAir * 10f, leftHandRunRef.transform.localPosition.z);

                desiredPos = new Vector3(Mathf.Sin(Time.time * runFrequency) * runYoffset * scopedMulti + 0.15f, -0.05f + Mathf.Sin(Time.time * runFrequency) * runYoffset * scopedMulti + xAir * scopedMulti * 3f, 0.1f);
                desiredRot = Quaternion.Euler(-70, Mathf.Sin(Time.time * runRotFrequency * scopedMulti) * runRotYOffset * scopedMulti, 0);
                */

                //old 2 hand
                desiredPos = new Vector3(Mathf.Sin(Time.time * runFrequency) * runYoffset * scopedMulti + -0.05f, -0.05f + Mathf.Sin(Time.time * runFrequency) * runYoffset * scopedMulti + xAir * scopedMulti * 3f, 0.1f);
                desiredRot = Quaternion.Euler(0, -70 + Mathf.Sin(Time.time * runRotFrequency * scopedMulti) * runRotYOffset * scopedMulti, 0);
            }
            //else
            //{
            //    if (leftHand.reference != PlayerController.main.GrabbedGun.leftHandPose)
            //    {
            //        leftHand.Grab(PlayerController.main.GrabbedGun.leftHandPose, 0.3f);
            //    }
            //}
        }
        //directionForce = Vector3.Lerp(directionForce, Vector3.zero, comeBackForce * Time.deltaTime);
        //orientationForce = Quaternion.Lerp(orientationForce, Quaternion.identity, comeBackForce * Time.deltaTime);
        //if (playerPose == PlayerPose.Run)
        //    leftHand.handTarget.transform.localPosition = Vector3.Lerp(leftHand.handTarget.transform.localPosition, leftHandDesiredPos, comeToForce * Time.deltaTime);
        poseTransform.localPosition = Vector3.Lerp(poseTransform.localPosition, desiredPos, comeToForce * Time.deltaTime);
        poseTransform.localRotation = Quaternion.Lerp( poseTransform.localRotation, desiredRot, comeToForce * Time.deltaTime);
    }
    public void SwitchPose(PlayerPose _pose)
    {
        playerPose = _pose;
        ApplyPoseConfiguration(GetPoseConfig(_pose));
    }

    public PlayerPoseConfig GetPoseConfig(PlayerPose _pose)
    {
        return configs[(int)_pose];
    }

    public void ApplyPoseConfiguration(PlayerPoseConfig _config)
    {
        leftHand.hint.localPosition = _config.leftHandHintPosition;
        rightHand.hint.localPosition = _config.rightHandHintPosition;
    }
}

public enum PlayerPose
{
    Idle,
    Walk,
    Run
}
[System.Serializable]
public class PlayerPoseConfig
{
    public PlayerPose playerPose = new PlayerPose();
    public Vector3 leftHandHintPosition;
    public Vector3 rightHandHintPosition;

    public PlayerPoseConfig() { }
}
