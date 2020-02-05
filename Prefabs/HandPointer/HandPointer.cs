using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.OvrHelpers.Prefabs.HandPointer
{
    [RequireComponent(typeof(LineRenderer))]
    public class HandPointer : MonoBehaviour
    {
        [SerializeField]
        public Cursor.Cursor Cursor;

        [SerializeField]
        public float MaxDistance = 10f;

        [SerializeField]
        public LayerMask LayerMask;

        [SerializeField]
        public float PointerLength = 1;

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

        private void Update()
        {
            _lineRenderer.SetPosition(0, this.transform.position);
            _lineRenderer.SetPosition(1, this.transform.position + (this.transform.forward * PointerLength));

            if (Cursor != null)
            {
                RaycastHit hit;

                Ray ray = new Ray(this.transform.position, this.transform.forward);

                if (Physics.Raycast(ray, out hit, MaxDistance, LayerMask))
                {
                    Cursor.Show(hit);
                }
                else
                {
                    Cursor.Hide();
                }
            }
        }
    }
}