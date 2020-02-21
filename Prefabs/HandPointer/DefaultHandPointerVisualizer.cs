using UnityEngine;

namespace FVTC.LearningInnovations.Unity.OvrHelpers.Prefabs.HandPointer
{
    [RequireComponent(typeof(LineRenderer))]
    public class DefaultHandPointerVisualizer : HandPointerVisualizer
    {
        [SerializeField]
        public Vector3 PointerStartOffset = Vector3.zero;

        [SerializeField]
        public float PointerLength = 2f;

        [SerializeField]
        public Material DefaultMaterial;

        [SerializeField]
        public Material ActiveMaterial;

        [SerializeField]
        public OVRInput.Button ActivityButton = OVRInput.Button.Any;

        LineRenderer _lineRenderer;

        void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();

            if (DefaultMaterial)
                _lineRenderer.material = DefaultMaterial;
            else
                DefaultMaterial = _lineRenderer.material;
        }

        public override void Visualize(HandPointerUpdateInfo pointerUpdateInfo)
        {
            base.Visualize(pointerUpdateInfo);

            Vector3 lineStart, lineEnd;

            lineStart = transform.position + transform.TransformDirection(PointerStartOffset);

            float startDistance = Vector3.Distance(transform.position, lineStart);

            if (pointerUpdateInfo.RaycastHit.HasValue && pointerUpdateInfo.RaycastHit.Value.distance < (PointerLength - startDistance))
            {
                lineEnd = pointerUpdateInfo.RaycastHit.Value.point;
            }
            else
            {
                lineEnd = lineStart + (transform.forward * PointerLength);
            }

            _lineRenderer.SetPosition(0, lineStart);
            _lineRenderer.SetPosition(1, lineEnd);

            _lineRenderer.material = OVRInput.Get(ActivityButton, pointerUpdateInfo.Pointer.Controller)
                ? ActiveMaterial
                : DefaultMaterial;
        }
    }
}