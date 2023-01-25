using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public bool isLeft;
    public Transform palmRoot;
    public Transform handTarget;
    public Transform hint;
    public List<Transform> bones = new List<Transform>();

    public HandPoseReference reference;
    private void Awake()
    {
        InitBones();
    }

    public void Grab(HandPoseReference _poseRef)
    {
        reference = _poseRef;
        handTarget.transform.parent = _poseRef.transform.parent;

        handTarget.localPosition = _poseRef.transform.localPosition;

        handTarget.localRotation = _poseRef.transform.localRotation;

        Pose(_poseRef);
    }

    public void Grab(HandPoseReference _poseRef, float _duration)
    {
        StartCoroutine(IGrab(_poseRef, _duration));
    }

    IEnumerator IGrab(HandPoseReference _poseRef, float _duration)
    {
        reference = _poseRef;
        handTarget.transform.parent = _poseRef.transform.parent;

        var p = handTarget.localPosition;
        var r = handTarget.localRotation;
        

        Pose(_poseRef, _duration);
        float t = 0;

        while (t < _duration)
        {
            handTarget.localPosition = Vector3.Lerp(p, _poseRef.transform.localPosition, t / _duration);
            handTarget.localRotation = Quaternion.Lerp(r, _poseRef.transform.localRotation, t / _duration);
            yield return null;
            t += Time.deltaTime;
        }

        handTarget.localPosition = _poseRef.transform.localPosition;
        handTarget.localRotation = _poseRef.transform.localRotation;
    }
    private void Update()
    {
        //if (reference != null)
        //    Pose(reference);
    }

    private void InitBones()
    {
        bones.AddRange(palmRoot.GetComponentsInChildren<Transform>());
    }

    public void Pose(HandPoseReference _poseRef)
    {
        reference = _poseRef;
        for (int i = 1; i < bones.Count; i++)
        {
            bones[i].localPosition = _poseRef.bones[i].localPosition;
            bones[i].localEulerAngles = _poseRef.bones[i].localEulerAngles;
        }

    }
    public void Pose(HandPoseReference _poseRef, float _duration)
    {
        StartCoroutine(IPose(_poseRef, _duration));

    }

    public IEnumerator IPose(HandPoseReference _poseRef, float _duration)
    {
        reference = _poseRef;

        List<Vector3> ps = new List<Vector3>();
        List<Quaternion> rs = new List<Quaternion>();

        foreach (var b in bones)
        {
            ps.Add(b.localPosition);
            rs.Add(b.localRotation);
        }

        float t = 0;
        while (t < _duration)
        {
            for (int i = 1; i < bones.Count; i++)
            {
                bones[i].localPosition = Vector3.Lerp(ps[i], _poseRef.bones[i].localPosition, t / _duration);
                bones[i].localRotation = Quaternion.Lerp(rs[i], _poseRef.bones[i].localRotation, t / _duration);
            }

            yield return null;
            t += Time.deltaTime;
        }
        
        for (int i = 1; i < bones.Count; i++)
        {
            bones[i].localPosition = _poseRef.bones[i].localPosition;
            bones[i].localRotation = _poseRef.bones[i].localRotation;
        }
    }
}
