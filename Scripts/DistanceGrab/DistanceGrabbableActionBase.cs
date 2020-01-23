using System;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.OvrHelpers.DistanceGrabbable
{
    [RequireComponent(typeof(OculusSampleFramework.DistanceGrabbable))]
    public class DistanceGrabbableActionBase : MonoBehaviour
    {

        OculusSampleFramework.DistanceGrabbable _distanceGrabbable;
        void Start()
        {
            _distanceGrabbable = GetComponent<OculusSampleFramework.DistanceGrabbable>();

        }


        private bool _wasGrabbing;

        private void Update()
        {
            bool isGrabbing = GetIsGrabbed();

            if (_wasGrabbing && isGrabbing)
            {
                OnGrabbing();
            }
            else if (_wasGrabbing && !isGrabbing)
            {
                OnGrabEnd();
            }
            else if (!_wasGrabbing && isGrabbing)
            {
                OnGrabStart();
            }

            _wasGrabbing = isGrabbing;
        }

        private bool GetIsGrabbed()
        {
            return _distanceGrabbable.isGrabbed;
        }

        protected virtual void OnGrabStart()
        {

        }

        protected virtual void OnGrabbing()
        {

        }

        protected virtual void OnGrabEnd()
        {

        }

    }
}