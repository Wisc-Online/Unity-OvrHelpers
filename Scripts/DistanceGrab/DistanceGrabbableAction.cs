using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FVTC.LearningInnovations.Unity.OvrHelpers.DistanceGrabbable {
    public class DistanceGrabbableAction : DistanceGrabbableActionBase
    {

        [SerializeField]
        public UnityEvent GrabStartAction;

        [SerializeField]
        public UnityEvent GrabEndAction;

        [SerializeField]
        public UnityEvent GrabbingAction;

        protected override void OnGrabbing()
        {
            base.OnGrabbing();

            GrabbingAction?.Invoke();
        }

        protected override void OnGrabStart()
        {
            base.OnGrabStart();

            GrabStartAction?.Invoke();
        }

        protected override void OnGrabEnd()
        {
            base.OnGrabEnd();

            GrabEndAction?.Invoke();
        }
    } 
}