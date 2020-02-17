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


        protected Pose _lastGrabberTargetPose;


        public override void GrabStart(DistanceGrabber grabber)
        {
            base.GrabStart(grabber);




            var target = Grabber.GetTarget();

            _lastGrabberTargetPose.position = target.position;
            _lastGrabberTargetPose.rotation = target.rotation;
        }


        public override void GrabUpdate()
        {
            base.GrabUpdate();

            Transform target = Grabber.GetTarget();

            GrabUpdatePosition(target);
            GrabUpdateRotation(target);

            _lastGrabberTargetPose.position = target.position;
            _lastGrabberTargetPose.rotation = target.rotation;
        }

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
            Vector3 deltaAngles = transform.forward - _lastGrabberTargetPose.forward;

            transform.forward = target.forward + deltaAngles;
        }

        protected virtual void GrabUpdateRotationSnapToTarget(Transform target)
        {
            GrabUpdateRotationSnapToTarget(GetTargetRotation(target));
        }

        private void GrabUpdateRotationSnapToTarget(Quaternion targetRotation)
        {
            transform.rotation = targetRotation;
        }

        protected virtual void GrabUpdateRotationTractorBeam(Transform target)
        {
            Quaternion targetRotation = GetTargetRotation(target);

            float degreesBetweenTargetAndGrabbable = Quaternion.Angle(targetRotation, transform.rotation);

            if (degreesBetweenTargetAndGrabbable <= TractorBeamTargetLockRotation)
            {
                GrabUpdateRotationSnapToTarget(targetRotation);
            }
            else
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * TractorBeamRotationSpeed);
            }
        }


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
            transform.position = target;
        }

        protected virtual void GrabUpdatePositionPreserveDistance(Transform target)
        {
            var distance = Vector3.Distance(transform.position, _lastGrabberTargetPose.position);

            transform.position = target.position + (target.forward * distance);
        }

        // used to indicate that the grabbable was at one point close enough to 
        // 'snap' to the target.  Therefore any subsequent updates should keep it
        // 'snapped' to the target.
        private bool _tractorBeamPositionSnapOverride = false;

        protected virtual void GrabUpdatePositionTractorBeam(Transform target)
        {
            Vector3 targetPosition = GetTargetPosition(target);

            if (_tractorBeamPositionSnapOverride || Vector3.Distance(targetPosition, transform.position) <= TractorBeamTargetLockDistance)
            {
                _tractorBeamPositionSnapOverride = true;
                GrabUpdatePositionSnapToTarget(targetPosition);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * TractorBeamMovementSpeed);
            }
        }

        private Vector3 GetTargetPosition(Transform target)
        {
            Vector3 worldSpaceOffsetPosition = target.TransformDirection(this.PositionOffset);

            Vector3 targetPosition = (target.position + worldSpaceOffsetPosition);
            return targetPosition;
        }

        public override void GrabEnd()
        {
            base.GrabEnd();

            // make sure to set this back so tractor-beaming works again.
            _tractorBeamPositionSnapOverride = false;
        }
    }
}