using System.Collections.Generic;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.OvrHelpers.DistanceGrabbable
{
    public abstract class DistanceGrabbableBase : MonoBehaviour
    {
        public DistanceGrabber Grabber { get; private set; }

        public int? OriginalLayer { get; private set; }

        public bool IsGrabbed { get { return Grabber != null; } }

        public virtual void GrabStart(DistanceGrabStartInfo grabInfo)
        {
            Grabber = grabInfo.Grabber;

            OriginalLayer = null;

            if (Grabber.GrabbingLayer != null)
            {
                MoveAllObjectsToLayer(Grabber.GrabbingLayer.layerIndex);
            }
        }

        private readonly Dictionary<GameObject, int> _originalLayers = new Dictionary<GameObject, int>();

        private void MoveAllObjectsToLayer(int layer)
        {
            _originalLayers.Clear();

            MoveObjectToLayer(gameObject, layer);
        }

        private void MoveAllObjectsBackToOriginalLayer()
        {
            foreach(var kvp in _originalLayers)
            {
                kvp.Key.layer = kvp.Value;
            }

            _originalLayers.Clear();
        }

        private void MoveObjectToLayer(GameObject gameObject, int layer)
        {
            _originalLayers[gameObject] = gameObject.layer;
            gameObject.layer = layer;

            for(int i = 0; i < gameObject.transform.childCount; ++i)
            {
                MoveObjectToLayer(gameObject.transform.GetChild(i).gameObject, layer);
            }
        }



        public virtual void GrabEnd()
        {
            if (OriginalLayer.HasValue)
            {
                this.gameObject.layer = OriginalLayer.Value;
            }

            this.Grabber = null;

            MoveAllObjectsBackToOriginalLayer();
        }

        public virtual void GrabUpdate()
        {
        }
    }
}