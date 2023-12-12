#if ASINK_TEST
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;

public class ViveOpenxrHandSubsystem : XRHandSubsystem
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void RegisterDescriptor()
    {
        var handsSubsystemCinfo = new XRHandSubsystemDescriptor.Cinfo
        {
            id = "Vive-Openxr-Hands",
            providerType = typeof(ViveOpenxrHandSubsystem),
            subsystemTypeOverride = typeof(ViveOpenxrHandSubsystem)
        };
        XRHandSubsystemDescriptor.Register(handsSubsystemCinfo);
    }
}

#endif