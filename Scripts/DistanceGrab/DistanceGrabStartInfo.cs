using UnityEngine;

namespace FVTC.LearningInnovations.Unity.OvrHelpers.DistanceGrabbable
{
    public class DistanceGrabStartInfo
    {
        public DistanceGrabStartInfo(DistanceGrabber grabber, Vector3 position)
        {
            this.Grabber = grabber;
            this.Position = position;
        }

        public DistanceGrabber Grabber { get; }
        public Vector3 Position { get; }
    }
}

