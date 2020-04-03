using FVTC.LearningInnovations.Unity.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.OvrHelpers.Prefabs.HandPointer
{
    public class HandPointer : MonoBehaviour
    {
        [SerializeField]
        public OVRInput.Controller Controller = OVRInput.Controller.None;

        [SerializeField]
        public OVRInput.Button LeftClickButton = OVRInput.Button.PrimaryIndexTrigger;

        [SerializeField]
        public OVRInput.Button RightClickButton = OVRInput.Button.PrimaryHandTrigger;

        [SerializeField]
        public Cursor.Cursor Cursor;

        [SerializeField]
        public float MaxDistance = 10f;

        [SerializeField]
        public LayerMask LayerMask;
        
        [SerializeField]
        public HandPointerVisualizer Visualizer;



        private void Start()
        {
            if (Cursor != null && Cursor.IsPrefab())
            {
                // must be a prefab, create an instance

                Cursor.Cursor cursorInstance = Instantiate<Cursor.Cursor>(this.Cursor);
                cursorInstance.Hide();

                this.Cursor = cursorInstance;

                
            }

            if (this.Cursor)
            {
                this.Cursor.Pointer = this;
            }

            if (Visualizer && Visualizer.IsPrefab())
            {
                HandPointerVisualizer v = Instantiate(this.Visualizer, this.transform);

                this.Visualizer = v;
            }
        }

        private void Update()
        {
            if (Cursor)
            {
                Cursor.Controller = this.Controller;
                Cursor.LeftClickButton = this.LeftClickButton;
                Cursor.RightClickButton = this.RightClickButton;
            }

            RaycastHit? nullHit = null;
            RaycastHit hit;

            Ray ray = new Ray(this.transform.position, this.transform.forward);

            if (Physics.Raycast(ray, out hit, MaxDistance, LayerMask))
            {
                nullHit = hit;

                if (Cursor)
                {
                    Cursor.Show(hit);
                }
            }
            else if (Cursor)
            {
                Cursor.Hide();
            }

            if (Visualizer)
            {
                Visualizer.Visualize(new HandPointerUpdateInfo(this, nullHit));
            }
        }
    }
}