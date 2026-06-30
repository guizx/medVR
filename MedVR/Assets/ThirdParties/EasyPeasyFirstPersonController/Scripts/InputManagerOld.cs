namespace EasyPeasyFirstPersonController
{
    using UnityEngine;

    public class InputManagerOld : MonoBehaviour, IInputManager
    {
        public Vector2 moveInput => new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        public Vector2 lookInput => new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        public bool jump => Input.GetKey(KeyCode.Space);
        public bool sprint => Input.GetKey(KeyCode.LeftShift);
        public bool crouch => Input.GetKey(KeyCode.LeftControl);
        public bool slide => Input.GetKey(KeyCode.LeftControl);
    }
}