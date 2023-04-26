using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wave.OpenXR.Toolkit;
using Wave.OpenXR.Toolkit.Hand;

namespace Wave.OpenXR.Samples.Hand
{
    public class Joint_Movement : MonoBehaviour
    {
        public int jointNum = 0;
        public bool isLeft = false;
        [SerializeField] List<GameObject> Childs = new List<GameObject>();

        void Update()
        {
            HandJoint joint = HandTracking.GetHandJointLocations(isLeft ? HandFlag.Left : HandFlag.Right)[jointNum];
            if (joint.isValid)
            {
                transform.localPosition = joint.position;
                transform.rotation = joint.rotation;
            }
            ActiveChilds(joint.isValid);
        }

        void ActiveChilds(bool _SetActive)
        {
            for (int i = 0; i < Childs.Count; i++)
            {
                Childs[i].SetActive(_SetActive);
            }
        }
    }

   
}
