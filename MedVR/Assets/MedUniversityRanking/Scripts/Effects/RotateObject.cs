using UnityEngine;

namespace Nato.GameFeel.Effect
{
    public class RotateObject : MonoBehaviour
    {
        public float speedRotation = 2;
        [SerializeField] private bool xAxis = true;
        [SerializeField] private bool yAxis;
        [SerializeField] private bool zAxis;

        private void Update()
        {
            if (xAxis)
                transform.Rotate(new Vector3(0, 180 * Time.deltaTime * speedRotation, 0));
            else if (yAxis)
                transform.Rotate(new Vector3(180 * Time.deltaTime * speedRotation, 0, 0));
            else if (zAxis)
                transform.Rotate(new Vector3(0, 0, 180 * Time.deltaTime * speedRotation));
        }

        public void SetSpeedRotation(float speedRotation)
        {
            this.speedRotation = speedRotation;
        }
    }
}