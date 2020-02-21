using UnityEngine;

namespace FVTC.LearningInnovations.Unity.OvrHelpers.DistanceGrabbable
{
    public class DistanceGrabber : MonoBehaviour
    {
        #region Settings Available in Editor
        [SerializeField]
        [Header("Grab Settings")]
        public OVRInput.Controller Controller = OVRInput.Controller.RTouch;

        [SerializeField]
        public OVRInput.Button Button = OVRInput.Button.PrimaryIndexTrigger;

        [SerializeField]
        public float MaxGrabDistance = 10f;

        [SerializeField]
        public LayerMask GrabLayerMask = ~0;

        [SerializeField]
        [Tooltip("The Layer the Grabbable will be changed while being grabbed.")]
        public Layer GrabbingLayer;

        [Header("Tractor Beam Settings")]
        [SerializeField]
        [Tooltip("The target to which the grabbable will move to.  If unset, grabbable will move/rotate to the this object's transform.")]
        public Transform TractorBeamTarget;

        [SerializeField]
        public OVRInput.Axis2D TractorBeamAxis = OVRInput.Axis2D.None;


        #endregion

        public DistanceGrabbableBase CurrentGrabbable { get; private set; }

        public bool IsGrabbing
        {
            get
            {
                return CurrentGrabbable != null;
            }
        }

        void Update()
        {
            if (OVRInput.Get(this.Button, this.Controller))
            {
                // the user is either starting, or continueing a grab

                var target = GetTarget();

                if (CurrentGrabbable != null)
                {
                    // continue grab
                    CurrentGrabbable.GrabUpdate();
                }
                else
                {
                    // check to initiate a grab
                    DistanceGrabbableBase grabbable;

                    Ray ray = new Ray(this.transform.position, this.transform.forward);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, this.MaxGrabDistance, this.GrabLayerMask))
                    {
                        grabbable = hit.transform.gameObject.GetComponentInParent<DistanceGrabbableBase>();

                        if (grabbable != null && !grabbable.IsGrabbed)
                        {
                            DistanceGrabStartInfo grabInfo = new DistanceGrabStartInfo(this, hit.point);

                            grabbable.GrabStart(grabInfo);

                            this.CurrentGrabbable = grabbable;
                        }
                    }
                }
            }
            else if (CurrentGrabbable != null)
            {
                // release the currently held object
                CurrentGrabbable.GrabEnd();
                CurrentGrabbable = null;
            }
        }

        public Transform GetTarget()
        {
            if (TractorBeamTarget)
                return TractorBeamTarget;

            return this.transform;
        }
    }
}