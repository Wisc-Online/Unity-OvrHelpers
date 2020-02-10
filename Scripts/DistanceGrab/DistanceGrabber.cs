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
        [Tooltip("The Layer Name the Grabbable will be changed while being grabbed.")]
        public string GrabbingLayerName;

        [Header("Tractor Beam Settings")]
        [SerializeField]
        [Tooltip("The target to which the grabbable will move to.  If unset, grabbable will move/rotate to the this object's transform.")]
        public Transform TractorBeamTarget;

        #endregion

        public DistanceGrabbable CurrentGrabbable { get; private set; }



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
                    DistanceGrabbable grabbable;

                    Ray ray = new Ray(this.transform.position, this.transform.forward);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, this.MaxGrabDistance, this.GrabLayerMask))
                    {
                        grabbable = hit.transform.gameObject.GetComponentInParent<DistanceGrabbable>();
                        
                        if (grabbable != null && !grabbable.IsGrabbed)
                        {
                            grabbable.GrabStart(this);

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