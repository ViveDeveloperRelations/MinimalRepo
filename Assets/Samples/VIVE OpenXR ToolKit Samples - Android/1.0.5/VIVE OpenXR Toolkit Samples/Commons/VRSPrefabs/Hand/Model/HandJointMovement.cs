using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wave.OpenXR.Toolkit.Hand;
using Wave.OpenXR.Toolkit;

public class HandJointMovement : MonoBehaviour
{
    public HandFlag Hand;
    [SerializeField] int Joint;
    void Start()
    {
        Joint = int.Parse(name.Split(new char[] { '_' })[1]);
    }

    void Update()
    {
        //transform.position = HandTracking.Get_HandJointLocations(Hand)[Joint].Position;
        transform.rotation = HandTracking.GetHandJointLocations(Hand)[Joint].rotation;
    }
}
