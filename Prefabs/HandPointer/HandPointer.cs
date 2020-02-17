using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.OvrHelpers.Prefabs.HandPointer
{
    [RequireComponent(typeof(LineRenderer))]
    public class HandPointer : MonoBehaviour
    {
        [SerializeField]
        public OVRInput.Controller Controller = OVRInput.Controller.None;

        [SerializeField]
        public OVRInput.Button PointerButton = OVRInput.Button.PrimaryIndexTrigger;

        [SerializeField]
        public Cursor.Cursor Cursor;

        [SerializeField]
        public float MaxDistance = 10f;

        [SerializeField]
        public LayerMask LayerMask;

        [SerializeField]
        public float PointerLength = 1;

        [SerializeField]
        public Vector3 PointerStartOffset = Vector3.zero;

        LineRenderer _lineRenderer;

        private void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();

            if (Cursor != null)
            {
                if (Cursor.gameObject.scene.name == null)
                {
                    // must be a prefab, create an instance

                    Cursor.Cursor cursorInstance = Instantiate<Cursor.Cursor>(this.Cursor);
                    cursorInstance.Hide();

                    this.Cursor = cursorInstance;
                }
            }
        }

        private void LateUpdate()
        {
            Vector3 lineStart, lineEnd;

            lineStart = this.transform.position + transform.TransformDirection(PointerStartOffset);
            lineEnd = lineStart + (transform.forward * PointerLength);

            _lineRenderer.SetPosition(0, lineStart);
            _lineRenderer.SetPosition(1, lineEnd);

            if (Cursor)
            {
                Cursor.Controller = this.Controller;
                Cursor.PointerButton = this.PointerButton;
            }
            RaycastHit hit;

            Ray ray = new Ray(this.transform.position, this.transform.forward);

            if (Physics.Raycast(ray, out hit, MaxDistance, LayerMask))
            {
                if (Cursor)
                {
                    Cursor.Show(hit);
                }

                if (hit.distance < PointerLength)
                {
                    _lineRenderer.SetPosition(1, this.transform.position + (this.transform.forward * hit.distance));
                }
            }
            else if (Cursor)
            {
                Cursor.Hide();
            }
        }
    }
}