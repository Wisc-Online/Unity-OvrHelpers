using UnityEngine;

namespace FVTC.LearningInnovations.Unity.OvrHelpers.Prefabs.HandPointer
{
    public class HandPointerUpdateInfo
    {
        public HandPointerUpdateInfo(HandPointer pointer, RaycastHit? raycastHit)
        {
            this.Pointer = pointer;
            this.RaycastHit = raycastHit;
        }

        public HandPointer Pointer { get; }
        public RaycastHit? RaycastHit { get; }
    }
}