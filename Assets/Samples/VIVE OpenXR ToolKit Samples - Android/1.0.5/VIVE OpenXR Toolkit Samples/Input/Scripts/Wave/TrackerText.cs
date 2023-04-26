// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Wave.OpenXR.Samples.OpenXRInput
{
    [RequireComponent(typeof(Text))]
    public class TrackerText : MonoBehaviour
    {
        const string LOG_TAG = "Wave.OpenXR.Samples.OpenXRInput.TrackerText";
        void DEBUG(string msg) { Debug.Log(LOG_TAG + msg); }

        #region Right Tracker
        [SerializeField]
        private InputActionReference m_TrackedR = null;
        public InputActionReference TrackedR { get => m_TrackedR; set => m_TrackedR = value; }

        [SerializeField]
        private InputActionReference m_TrackingStateR = null;
        public InputActionReference TrackingStateR { get => m_TrackingStateR; set => m_TrackingStateR = value; }

        [SerializeField]
        private InputActionReference m_RightA = null;
        public InputActionReference RightA { get => m_RightA; set => m_RightA = value; }
        #endregion

        #region Left Tracker
        [SerializeField]
        private InputActionReference m_TrackedL = null;
        public InputActionReference TrackedL { get => m_TrackedL; set => m_TrackedL = value; }

        [SerializeField]
        private InputActionReference m_TrackingStateL = null;
        public InputActionReference TrackingStateL { get => m_TrackingStateL; set => m_TrackingStateL = value; }

        [SerializeField]
        private InputActionReference m_LeftX = null;
        public InputActionReference LeftX { get => m_LeftX; set => m_LeftX = value; }

        [SerializeField]
        private InputActionReference m_LeftMenu = null;
        public InputActionReference LeftMenu { get => m_LeftMenu; set => m_LeftMenu = value; }
        #endregion

        bool getButton(InputActionReference actionReference)
        {
            if (OpenXRHelper.VALIDATE(actionReference, out string value))
            {
                if (actionReference.action.activeControl.valueType == typeof(bool))
                    return actionReference.action.ReadValue<bool>();
                if (actionReference.action.activeControl.valueType == typeof(float))
                    return actionReference.action.ReadValue<float>() > 0;
            }

            return false;
        }
        int getInteger(InputActionReference actionReference)
        {
            if (OpenXRHelper.VALIDATE(actionReference, out string value))
            {
                if (actionReference.action.activeControl.valueType == typeof(int))
                    return actionReference.action.ReadValue<int>();
            }

            return 0;
        }

        private Text m_Text = null;

        private void Start()
        {
            m_Text = GetComponent<Text>();
        }
        private void Update()
        {
            if (m_Text == null) { return; }

            // Left tracker text
            m_Text.text = "Left Tracker ";

            bool tracked = getButton(m_TrackedL);
            m_Text.text += "tracked: " + tracked + ", ";
            int trackingState = getInteger(m_TrackingStateL);
            m_Text.text += "state: " + trackingState + ", ";

            if (getButton(m_LeftX))
            {
                DEBUG("Update() Left X is pressed.");
                m_Text.text += "Left X";
            }
            if (getButton(m_LeftMenu))
            {
                DEBUG("Update() Left Menu is pressed.");
                m_Text.text += "Left Menu";
            }

            // Right tracker text
            m_Text.text += "\nRight Tracker ";

            tracked = getButton(m_TrackedR);
            m_Text.text += "tracked: " + tracked + ", ";
            trackingState = getInteger(m_TrackingStateR);
            m_Text.text += "state: " + trackingState + ", ";

            if (getButton(m_RightA))
            {
                DEBUG("Update() Right A is pressed.");
                m_Text.text += "Right A";
            }
        }
    }
}
