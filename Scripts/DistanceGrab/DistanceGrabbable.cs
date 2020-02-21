using System;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.OvrHelpers.DistanceGrabbable
{
    public class DistanceGrabbable : DistanceGrabbableBase
    {
        [Header("Grab Settings")]
        [SerializeField]
        public DistanceGrabbablePositionBehavior PositionBehavior = DistanceGrabbablePositionBehavior.TractorBeam;

        [SerializeField]
        public DistanceGrabbableRotationBehavior RotationBehavior = DistanceGrabbableRotationBehavior.PreserveOrientation;

        [SerializeField]
        [Tooltip("The root object to 'grab' when this object is grabbed by the user.")]
        public GameObject RootObject;

        [SerializeField]
        [Tooltip("The object that the grabber should appear to be holding.")]
        public GameObject Handle;

        [Header("Tractor Beam Settings")]
        [SerializeField]
        [Tooltip("Speed (units/second) the Grabbable will move toward the Tractor Beam Target's position.")]
        public float TractorBeamMovementSpeed = 1f;

        [SerializeField]
        [Tooltip("Speed (degrees/second) the Grabbable will rotate toward the Tractor Beam Target's orientation.")]
        public float TractorBeamRotationSpeed = 360f;

        [SerializeField]
        [Tooltip("Distance the Grabbable will \"snap\" to the Tractor Beam Target's position.")]
        public float TractorBeamTargetLockDistance = 0.5f;

        [SerializeField]
        [Tooltip("Degrees the Grabbable will \"snap\" to the Tractor Beam Target's orientation.")]
        public float TractorBeamTargetLockRotation = 15f;


        [Header("Snap Settings")]
        [SerializeField]
        public Vector3 PositionOffset;

        [SerializeField]
        public Vector3 RotationOffset;


        private Pose _lastGrabberTargetPose;
        private Vector3 _grabOffset = Vector3.zero;

        private Pose _tractorBeamSnapOffset = Pose.identity;

        Transform Root
        {
            get
            {
                return RootObject ? RootObject.transform : transform;
            }
        }

        public override void GrabStart(DistanceGrabStartInfo grabStartInfo)
        {
            base.GrabStart(grabStartInfo);

            var target = Grabber.GetTarget();

            _lastGrabberTargetPose.position = target.position;
            _lastGrabberTargetPose.rotation = target.rotation;

            if (Handle)
            {
                _grabOffset = Root.position - Handle.transform.position;
            }
            else
            {
                _grabOffset = Root.position - grabStartInfo.Position;
            }
        }

        public override void GrabUpdate()
        {
            base.GrabUpdate();

            Transform target = Grabber.GetTarget();


            UpdateTractorBeamOffset();

            GrabUpdatePosition(target);
            GrabUpdateRotation(target);

            _lastGrabberTargetPose.position = target.position;
            _lastGrabberTargetPose.rotation = target.rotation;
            
        }

        private void UpdateTractorBeamOffset()
        {
            if (this.Grabber.TractorBeamAxis != OVRInput.Axis2D.None)
            {
                Vector2 axes = OVRInput.Get(this.Grabber.TractorBeamAxis, this.Grabber.Controller);
                UpdateTractorBeamOffsetPosition(axes);
                UpdateTractorBeamOffsetRotation(axes);
            }
        }

        private void UpdateTractorBeamOffsetRotation(Vector2 axes)
        {
            if (axes.x != 0)
            {
                float yRotation = axes.x * TractorBeamRotationSpeed * Time.deltaTime;

                Quaternion rotation = _tractorBeamSnapOffset.rotation;

                _tractorBeamSnapOffset.rotation *= Quaternion.Euler(0, yRotation, 0);

                _tractorBeamSnapOffset.rotation = rotation;
            }
        }

        private void UpdateTractorBeamOffsetPosition(Vector2 axes)
        {
            if (axes.y != 0)
            {
                float zOffset = axes.y * TractorBeamMovementSpeed * Time.deltaTime;

                Vector3 pos = _tractorBeamSnapOffset.position;

                pos.z += zOffset;

                _tractorBeamSnapOffset.position = pos;
            }
        }

        #region GrabUpdateRotation

        protected virtual void GrabUpdateRotation(Transform target)
        {
            switch (this.RotationBehavior)
            {
                case DistanceGrabbableRotationBehavior.PreserveOrientation:
                    GrabUpdateRotationPreserveOrientation(target);
                    break;
                case DistanceGrabbableRotationBehavior.SnapToTarget:
                    GrabUpdateRotationSnapToTarget(target);
                    break;
                case DistanceGrabbableRotationBehavior.TractorBeam:
                default:
                    GrabUpdateRotationTractorBeam(target);
                    break;
            }
        }

        protected virtual void GrabUpdateRotationPreserveOrientation(Transform target)
        {
            Vector3 deltaAngles = Root.forward - _lastGrabberTargetPose.forward;

            Root.forward = target.forward + deltaAngles;
        }

        protected virtual void GrabUpdateRotationSnapToTarget(Transform target)
        {
            GrabUpdateRotationSnapToTarget(GetTargetRotation(target));
        }

        private void GrabUpdateRotationSnapToTarget(Quaternion targetRotation)
        {
            Root.rotation = targetRotation;
        }

        bool _tractorBeamRotationSnapOverride;
        protected virtual void GrabUpdateRotationTractorBeam(Transform target)
        {
            Quaternion targetRotation = GetTargetRotation(target);

            if (_tractorBeamRotationSnapOverride || Quaternion.Angle(targetRotation, Root.rotation) <= TractorBeamTargetLockRotation)
            {
                _tractorBeamRotationSnapOverride = true;
                GrabUpdateRotationSnapToTarget(targetRotation);
            }
            else
            {
                Root.rotation = Quaternion.RotateTowards(Root.rotation, targetRotation, Time.deltaTime * TractorBeamRotationSpeed);
            }
        }

        #endregion


        private Quaternion GetTargetRotation(Transform target)
        {
            Quaternion targetRotation = target.rotation;

            if (RotationOffset != Vector3.zero)
            {
                Quaternion offsetRotation = Quaternion.Euler(RotationOffset);

                targetRotation = targetRotation * offsetRotation;
            }

            return targetRotation;
        }

        #region Grab Update Position

        protected virtual void GrabUpdatePosition(Transform target)
        {
            switch (this.PositionBehavior)
            {
                case DistanceGrabbablePositionBehavior.PreserveDistance:
                    GrabUpdatePositionPreserveDistance(target);
                    break;
                case DistanceGrabbablePositionBehavior.SnapToTarget:
                    GrabUpdatePositionSnapToTarget(target);
                    break;
                case DistanceGrabbablePositionBehavior.TractorBeam:
                default:
                    GrabUpdatePositionTractorBeam(target);
                    break;
            }
        }


        protected virtual void GrabUpdatePositionSnapToTarget(Transform target)
        {
            GrabUpdatePositionSnapToTarget(GetTargetPosition(target));
        }

        protected virtual void GrabUpdatePositionSnapToTarget(Vector3 target)
        {
            Root.position = target;
        }

        protected virtual void GrabUpdatePositionPreserveDistance(Transform target)
        {
            var distance = Vector3.Distance(Root.position, _lastGrabberTargetPose.position);

            var delta = target.TransformVector(_tractorBeamSnapOffset.position);

            Root.position = target.position + (target.forward * distance) + delta;
        }

        // used to indicate that the grabbable was at one point close enough to 
        // 'snap' to the target.  Therefore any subsequent updates should keep it
        // 'snapped' to the target.
        private bool _tractorBeamPositionSnapOverride = false;

        protected virtual void GrabUpdatePositionTractorBeam(Transform target)
        {
            Vector3 targetPosition = GetTargetPosition(target);

            if (_tractorBeamPositionSnapOverride || Vector3.Distance(targetPosition, Root.position) <= TractorBeamTargetLockDistance)
            {
                _tractorBeamPositionSnapOverride = true;
                GrabUpdatePositionSnapToTarget(targetPosition);
            }
            else
            {
                Root.position = Vector3.MoveTowards(Root.position, targetPosition, Time.deltaTime * TractorBeamMovementSpeed);
            }
        }

        #endregion

        private Vector3 GetTargetPosition(Transform target)
        {
            Vector3 worldSpaceOffsetPosition = target.TransformDirection(this.PositionOffset);

            Vector3 delta = target.TransformVector(_tractorBeamSnapOffset.position);

            Vector3 targetPosition = (target.position + worldSpaceOffsetPosition + delta) + _grabOffset;

            return targetPosition;
        }

        public override void GrabEnd()
        {
            base.GrabEnd();

            // make sure to set this back so tractor-beaming works again.
            _tractorBeamPositionSnapOverride = false;
            _tractorBeamRotationSnapOverride = false;
            _tractorBeamSnapOffset = Pose.identity;

        }
    }
}