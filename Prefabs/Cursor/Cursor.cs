using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FVTC.LearningInnovations.Unity.OvrHelpers.Prefabs.Cursor
{
    public class Cursor : MonoBehaviour
    {
        
        public virtual void Hide()
        {
            this.gameObject.SetActive(false);
        }

        public virtual void Show(RaycastHit hit)
        {
            transform.position = hit.point;
            transform.LookAt(hit.point + hit.normal);

            this.gameObject.SetActive(true);
        }
    }
}