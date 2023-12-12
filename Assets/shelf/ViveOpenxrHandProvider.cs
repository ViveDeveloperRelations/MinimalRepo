#if ASINK_TEST
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
    private ulong handTracker = 0;

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
        bool isTrackingLeft = viveHands.GetJointLocations(isLeft: true, handJointLocation: out XrHandJointLocationEXT[] handJointLocationLeft);
        bool isTrackingRight = viveHands.GetJointLocations(isLeft: true,handJointLocation: out XrHandJointLocationEXT[] handJointLocationRight);
        
        leftHandIsTracked = isTrackingLeft;
        rightHandIsTracked = isTrackingRight;
        ++numStartCalls;
    }

    public override void Stop()
    {
        //TODO: check to see if I should be destroying it or if it is appropriate to keep it loaded and let openxr manage it. 
        //XR_EXT_hand_tracking.Interop.xrDestroyHandTrackerEXT()
        ++numStopCalls;
    }

    public override void Destroy()
    {
        ++numDestroyCalls;
    }

    public override void GetHandLayout(NativeArray<bool> jointsInLayout)
    {
        ++numGetHandLayoutCalls;
        
        //FIXME: provide actual check instead of relying on our openxr layout
        //the structure is based on openxr joints and we implment them all based on VIVE.OpenXR.Hand.XrHandJointEXT. this implicitly assumes our openxr version is the same as the one used by the hands subsystem
        for (int jointIndex = 0; jointIndex < jointsInLayout.Length; ++jointIndex)
            jointsInLayout[jointIndex] = true;
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
        
         viveHands.GetJointLocations(isLeft: true, handJointLocation: out XrHandJointLocationEXT[] handJointLocationLeft);
         var feature = OpenXRSettings.Instance.GetFeature<ViveHandTracking>();
         //feature.
         //feature.
         
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


#endif