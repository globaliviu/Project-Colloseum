using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPoseReference : MonoBehaviour
{
    public bool isLeft;

    public List<Transform> bones = new List<Transform>();

    public void Awake()
    {
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
    }

}
