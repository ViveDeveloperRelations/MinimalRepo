using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.ProviderImplementation;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.ProviderImplementation;
using UnityEngine.XR.OpenXR;
using VIVE.OpenXR;
using VIVE.OpenXR.Hand;

[Preserve]
class ViveOpenxrHandProvider : XRHandSubsystemProvider
{
    private ViveHandTracking feature = null;
    private XR_EXT_hand_tracking_defs viveHands => XR_EXT_hand_tracking.Interop;
    
    public int numStartCalls { get; private set; }
    public int numStopCalls { get; private set; }
    public int numDestroyCalls { get; private set; }
    public int numGetHandLayoutCalls { get; private set; }
    public int numTryUpdateHandsCalls { get; private set; }
    public XRHandSubsystem.UpdateType mostRecentUpdateType { get; private set; }

    public bool leftHandIsTracked { get; set; } = true;
    
    public bool rightHandIsTracked { get; set; } = true;
    
    public override void Start()
    {
        bool isTrackingLeft = XR_EXT_hand_tracking.Interop.GetJointLocations(isLeft: true, handJointLocation: out XrHandJointLocationEXT[] handJointLocationLeft);
        bool isTrackingRight = XR_EXT_hand_tracking.Interop.GetJointLocations(isLeft: true,handJointLocation: out XrHandJointLocationEXT[] handJointLocationRight);
        
        leftHandIsTracked = isTrackingLeft;
        rightHandIsTracked = isTrackingRight;
        ++numStartCalls;
    }

    public override void Stop()
    {
        ++numStopCalls;
    }

    public override void Destroy()
    {
        ++numDestroyCalls;
    }

    public override void GetHandLayout(NativeArray<bool> jointsInLayout)
    {
        ++numGetHandLayoutCalls;
        for (int jointIndex = 0; jointIndex < jointsInLayout.Length; ++jointIndex)
            jointsInLayout[jointIndex] = TestHandData.jointsInLayout[jointIndex];
    }

    public override XRHandSubsystem.UpdateSuccessFlags TryUpdateHands(
        XRHandSubsystem.UpdateType updateType,
        ref Pose leftHandRootPose,
        NativeArray<XRHandJoint> leftHandJoints,
        ref Pose rightHandRootPose,
        NativeArray<XRHandJoint> rightHandJoints)
    {
        mostRecentUpdateType = updateType;
        ++numTryUpdateHandsCalls;

        leftHandRootPose = TestHandData.leftRoot;
        rightHandRootPose = TestHandData.rightRoot;
        for (int jointIndex = 0; jointIndex < TestHandData.jointsInLayout.Length; ++jointIndex)
        {
            if (!TestHandData.jointsInLayout[jointIndex])
                continue;

            leftHandJoints[jointIndex] = XRHandProviderUtility.CreateJoint(
                Handedness.Left,
                XRHandJointTrackingState.Pose,
                XRHandJointIDUtility.FromIndex(jointIndex),
                TestHandData.leftHand[jointIndex]);

            rightHandJoints[jointIndex] = XRHandProviderUtility.CreateJoint(
                Handedness.Right,
                XRHandJointTrackingState.Pose,
                XRHandJointIDUtility.FromIndex(jointIndex),
                TestHandData.rightHand[jointIndex]);
        }

        var successFlags = XRHandSubsystem.UpdateSuccessFlags.All;
        
        if (!leftHandIsTracked)
            successFlags &= ~XRHandSubsystem.UpdateSuccessFlags.LeftHandJoints & ~XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose;
        
        if (!rightHandIsTracked)
            successFlags &= ~XRHandSubsystem.UpdateSuccessFlags.RightHandJoints & ~XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose;

        return successFlags;
    }
    
    public static string descriptorId => "Vive-Openxr-Hands"; //TODO: ensure this needs to be the same as the hands subsystem id
}



/*
public class ViveOpenxrHandProvider : XRHandSubsystemProvider
{
    public override RuntimeEnableMode GetAvailability()
    {
        return RuntimeEnableMode.Enabled;
    }

    public override bool Running { get; set; }

    public override void Start()
    {
        Running = true;
    }

    public override void Stop()
    {
        Running = false;
    }

    public override void UpdateTrackingData(XRHand hand, List<XRHandJoint> joints)
    {
    }
}
*/
