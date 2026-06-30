using UnityEngine;

namespace CC
{
    public class Cursor_Manager : MonoBehaviour
    {
        public static Cursor_Manager instance;

        public Texture2D cursorTexture;
        private Vector2 hotSpot = new Vector2(0, 0);

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            if (cursorTexture != null)
            {
                hotSpot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
                setDefaultCursor();
            }
        }

        public void setCursor(Texture2D texture)
        {
            if (cursorTexture != null)
            {
                Cursor.SetCursor(texture, hotSpot, CursorMode.Auto);
            }
        }

        public void setDefaultCursor()
        {
            setCursor(cursorTexture);
        }
    }
}