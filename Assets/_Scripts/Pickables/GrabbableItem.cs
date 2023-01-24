using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GrabbableItem : MonoBehaviour
{
    public HandPoseReference leftHandPose;
    public HandPoseReference rightHandPose;

    public UnityAction onEquip;
    
    

    public void OnEquip()
    {
        if(onEquip != null)
            onEquip.Invoke();
    }
    
}
