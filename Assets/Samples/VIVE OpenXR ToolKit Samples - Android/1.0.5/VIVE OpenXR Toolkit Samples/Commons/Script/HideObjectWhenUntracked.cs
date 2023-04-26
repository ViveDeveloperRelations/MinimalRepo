// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wave.OpenXR.Toolkit.Samples
{
    public class HideObjectWhenUntracked : MonoBehaviour
    {
        const string LOG_TAG = "Wave.OpenXR.Toolkit.Samples.HideObjectWhenUntracked";
        void DEBUG(string msg)
        {
            Debug.Log(LOG_TAG + " " + gameObject.name + (IsLeft ? " Left" : " Right") + ", " + msg);
        }

        public bool IsLeft = false;

        [SerializeField]
        private InputActionReference m_IsActive;
        public InputActionReference IsActive { get => m_IsActive; set => m_IsActive = value; }

        [SerializeField]
        private InputActionReference m_TrackingState;
        public InputActionReference TrackingState { get => m_TrackingState; set => m_TrackingState = value; }

        [SerializeField]
        private GameObject m_ObjectToHide = null;
        public GameObject ObjectToHide { get { return m_ObjectToHide; } set { m_ObjectToHide = value; } }

        int printFrame = 0;
        protected bool printIntervalLog = false;

        bool isActive = false;
        int trackingState = 0;
        bool positionTracked = false, rotationTracked = false;
        private void Update()
        {
            printFrame++;
            printFrame %= 300;
            printIntervalLog = (printFrame == 0);

            if (m_ObjectToHide == null) { return; }

            string errMsg = "";
            if (OpenXRHelper.VALIDATE(m_IsActive, out errMsg))
            {
                if (m_IsActive.action.activeControl.valueType == typeof(float))
                    isActive = m_IsActive.action.ReadValue<float>() > 0;
                if (m_IsActive.action.activeControl.valueType == typeof(bool))
                    isActive = m_IsActive.action.ReadValue<bool>();
            }
            else
            {
                isActive = false;
                if (printIntervalLog) { DEBUG("Update() " + m_IsActive.action.name + ", " + errMsg); }
            }
            if (OpenXRHelper.VALIDATE(m_TrackingState, out errMsg))
            {
                trackingState = m_TrackingState.action.ReadValue<int>();
            }
            else
            {
                trackingState = 0;
                if (printIntervalLog) { DEBUG("Update() " + m_TrackingState.action.name + ", " + errMsg); }
            }

            if (printIntervalLog)
                DEBUG("Update() isActive: " + isActive + ", trackingState: " + trackingState);

            positionTracked = ((UInt64)trackingState & (UInt64)XrSpaceLocationFlags.XR_SPACE_LOCATION_POSITION_TRACKED_BIT) != 0;
            rotationTracked = ((UInt64)trackingState & (UInt64)XrSpaceLocationFlags.XR_SPACE_LOCATION_ORIENTATION_TRACKED_BIT) != 0;

            bool tracked = isActive && positionTracked && rotationTracked;
            m_ObjectToHide.SetActive(tracked);
        }
    }
}
