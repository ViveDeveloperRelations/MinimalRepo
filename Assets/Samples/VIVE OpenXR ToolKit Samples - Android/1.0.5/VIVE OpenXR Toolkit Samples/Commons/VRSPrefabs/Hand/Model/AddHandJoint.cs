using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wave.OpenXR.Toolkit.Hand;
using Wave.OpenXR.Toolkit;

public class AddHandJoint : MonoBehaviour
{
    [SerializeField] HandFlag Hand;
    void Start()
    {
        HandJointMovement _Temp = gameObject.AddComponent<HandJointMovement>();
        _Temp.Hand = Hand;
        AddHJMScript(gameObject);
    }

    void Update()
    {
        transform.localPosition = HandTracking.GetHandJointLocations(Hand)[1].position;
    }

    void AddHJMScript(GameObject _GObj)
    {
        for(int i = 0; i < _GObj.transform.childCount; i++)
        {
            HandJointMovement _Temp = _GObj.transform.GetChild(i).gameObject.AddComponent<HandJointMovement>();
            _Temp.Hand = Hand;
            AddHJMScript(_GObj.transform.GetChild(i).gameObject);
        }
    }
}
