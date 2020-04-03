using UnityEngine.EventSystems;

namespace FVTC.LearningInnovations.Unity.OvrHelpers.Prefabs.Cursor
{
    public class CursorPointerEventData : PointerEventData
    {
        public CursorPointerEventData(Cursor cursor, EventSystem eventSystem) : base(eventSystem)
        {
            this.Cursor = cursor;
        }

        public Cursor Cursor { get; }
    }
}