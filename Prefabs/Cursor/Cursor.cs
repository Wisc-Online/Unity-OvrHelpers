using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FVTC.LearningInnovations.Unity.OvrHelpers.Prefabs.Cursor
{
    public class Cursor : MonoBehaviour
    {
        [SerializeField]
        public OVRInput.Controller Controller = OVRInput.Controller.None;

        [SerializeField]
        public OVRInput.Button LeftClickButton = OVRInput.Button.PrimaryHandTrigger;

        [SerializeField]
        public OVRInput.Button RightClickButton = OVRInput.Button.None;


        GameObject _target;
        bool isLeftButtonDown, isRightButtonDown;

        void Update()
        {
            if (_target && Controller != OVRInput.Controller.None)
            {
                HandleButtonEvents(LeftClickButton, PointerEventData.InputButton.Left, ref isLeftButtonDown);

                HandleButtonEvents(RightClickButton, PointerEventData.InputButton.Right, ref isRightButtonDown);
            }
        }

        private void HandleButtonEvents(OVRInput.Button button, PointerEventData.InputButton mouseButton, ref bool isButtonDown)
        {
            if (button != OVRInput.Button.None && Controller != OVRInput.Controller.None)
            {

                PointerEventData e = new PointerEventData(EventSystem.current);

                e.button = mouseButton;

                if (OVRInput.GetDown(button, this.Controller))
                {
                    OnPointerDown(_target, e);

                    isButtonDown = true;
                }

                if (OVRInput.GetUp(button, this.Controller))
                {
                    OnPointerUp(_target, e);

                    if (isButtonDown)
                    {
                        OnPointerClick(_target, e);
                    }

                    isButtonDown = false;
                }
            }
        }

        public virtual void Hide()
        {
            this.gameObject.SetActive(false);

            if (_target != null)
            {
                PointerEventData e = new PointerEventData(EventSystem.current);

                OnPointerExit(_target, e);
            }

            _target = null;
            isLeftButtonDown = false;
            isRightButtonDown = false;
        }

        public virtual void Show(RaycastHit hit)
        {
            transform.position = hit.point;
            transform.LookAt(hit.point + hit.normal);

            this.gameObject.SetActive(true);

            if (hit.collider && hit.collider.gameObject)
            {
                GameObject target = hit.collider.gameObject;

                PointerEventData e = new PointerEventData(EventSystem.current);

                if (!_target)
                {
                    OnPointerEnter(target, e);
                }
                else if (_target != target)
                {
                    OnPointerExit(_target, e);
                    OnPointerEnter(target, e);
                }

                _target = target;
            }
        }

        private void OnPointerEnter(GameObject target, PointerEventData e)
        {
            var handlers = target.GetComponentsInParent<IPointerEnterHandler>(includeInactive: false);

            if (handlers != null)
            {
                for(int i = 0; i < handlers.Length; ++i)
                {
                    handlers[i].OnPointerEnter(e);
                }
            }
        }

        private void OnPointerExit(GameObject target, PointerEventData e)
        {
            var handlers = target.GetComponentsInParent<IPointerExitHandler>(includeInactive: false);

            if (handlers != null)
            {
                for (int i = 0; i < handlers.Length; ++i)
                {
                    handlers[i].OnPointerExit(e);
                }
            }
        }

        private void OnPointerClick(GameObject target, PointerEventData e)
        {
            var handlers = target.GetComponentsInParent<IPointerClickHandler>(includeInactive: false);

            if (handlers != null)
            {
                for (int i = 0; i < handlers.Length; ++i)
                {
                    handlers[i].OnPointerClick(e);
                }
            }
        }

        private void OnPointerUp(GameObject target, PointerEventData e)
        {
            var handlers = target.GetComponentsInParent<IPointerUpHandler>(includeInactive: false);

            if (handlers != null)
            {
                for (int i = 0; i < handlers.Length; ++i)
                {
                    handlers[i].OnPointerUp(e);
                }
            }
        }

        private void OnPointerDown(GameObject target, PointerEventData e)
        {
            var handlers = target.GetComponentsInParent<IPointerDownHandler>(includeInactive: false);

            if (handlers != null)
            {
                for (int i = 0; i < handlers.Length; ++i)
                {
                    handlers[i].OnPointerDown(e);
                }
            }
        }

    }
}