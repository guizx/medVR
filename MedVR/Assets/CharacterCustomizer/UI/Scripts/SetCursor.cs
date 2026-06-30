using UnityEngine;
using UnityEngine.EventSystems;

namespace CC
{
    public class SetCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Texture2D cursorTexture;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Cursor_Manager.instance.setCursor(cursorTexture);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Cursor_Manager.instance.setDefaultCursor();
        }
    }
}