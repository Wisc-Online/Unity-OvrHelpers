using System;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.OvrHelpers.DistanceGrabbable
{
    public class DistanceGrabbable : MonoBehaviour
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


        public DistanceGrabber Grabber { get; private set; }

        public int? OriginalLayer { get; private set; }

        public bool IsGrabbed { get { return Grabber != null; } }

        public virtual void GrabStart(DistanceGrabber grabber)
        {
            this.Grabber = grabber;

            OriginalLayer = null;

            if (!string.IsNullOrWhiteSpace(grabber.GrabbingLayerName))
            {
                int grabbingLayer = LayerMask.NameToLayer(grabber.GrabbingLayerName);

                if (grabbingLayer > -1)
                {
                    this.OriginalLayer = this.gameObject.layer;

                    this.gameObject.layer = grabbingLayer;
                }
            }

            var target = Grabber.GetTarget();

            _lastGrabberTargetPose.position = target.position;
            _lastGrabberTargetPose.rotation = target.rotation;
        }

        Pose _lastGrabberTargetPose;

        public virtual void GrabUpdate()
        {
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
            transform.rotation = target.transform.rotation;
        }

        protected virtual void GrabUpdateRotationTractorBeam(Transform target)
        {
            float degreesBetweenTargetAndGrabbable = Vector3.Angle(transform.forward, target.transform.forward);

            if (degreesBetweenTargetAndGrabbable <= TractorBeamTargetLockRotation)
            {
                GrabUpdateRotationSnapToTarget(target);
            }
            else
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target.transform.rotation, Time.deltaTime * TractorBeamRotationSpeed);
            }
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
            transform.position = target.transform.position;
        }

        protected virtual void GrabUpdatePositionPreserveDistance(Transform target)
        {
            var distance = Vector3.Distance(transform.position, _lastGrabberTargetPose.position);

            transform.position = target.position + (target.forward * distance);
        }

        protected virtual void GrabUpdatePositionTractorBeam(Transform target)
        {
            var distanceFromTargetToGrabbable = (transform.position - target.position).magnitude;

            if (distanceFromTargetToGrabbable <= TractorBeamTargetLockDistance)
            {
                GrabUpdatePositionSnapToTarget(target);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * TractorBeamMovementSpeed);
            }
        }

        public virtual void GrabEnd()
        {
            if (OriginalLayer.HasValue)
            {
                this.gameObject.layer = OriginalLayer.Value;
            }

            this.Grabber = null;
        }
    }
}