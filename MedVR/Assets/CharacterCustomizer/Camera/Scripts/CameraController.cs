using UnityEngine;
using UnityEngine.EventSystems;

namespace CC
{
    [DefaultExecutionOrder(100)]
    public class CameraController : MonoBehaviour
    {
        public static CameraController instance;

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
        }

        public float ZoomMin = -0.6f;
        public float ZoomMax = -3.1f;
        public float ZoomPanScale = 0.5f;
        public float zoomTarget = 0.5f;

        private Camera _camera;
        private Transform cameraRoot;

        private Vector3 mouseOldPos;
        private Vector3 mouseDelta;

        private Vector3 cameraRotationTarget = new Vector3(10, -5, 0);
        private Vector3 cameraRotationDefault;
        public float rotateSpeed = 5;
        private bool dragging = false;

        private Vector3 cameraOffset;
        private Vector3 panOffset;
        public Vector3 cameraOffsetMin = new Vector3(-0.15f, 0.5f, 0);
        public Vector3 cameraOffsetMax = new Vector3(-0.3f, -0.1f, 0);

        public float panSpeed = 3;

        public float defaultHeadLevel = 1.8f;
        private float headAdjust = 0f;
        public GameObject headLevelObject;

        private bool panning = false;

        private void Start()
        {
            _camera = GetComponentInChildren<Camera>(true);

            cameraRoot = gameObject.transform;

            cameraRotationDefault = cameraRoot.localRotation.eulerAngles;
            cameraRotationTarget = cameraRotationDefault;
        }

        public void resetCamera()
        {
            cameraRotationTarget = cameraRotationDefault;
            panOffset = Vector3.zero;
        }

        private void setHeadLevel()
        {
            if (headLevelObject == null || !headLevelObject.activeInHierarchy)
            {
                headLevelObject = GameObject.FindGameObjectWithTag("HeadLevel");
            }
            if (headLevelObject == null)
            {
                headAdjust = 0f;
                return;
            }
            headAdjust = defaultHeadLevel - (headLevelObject.transform.position.y - transform.parent.position.y);
        }

        private void LateUpdate()
        {
            setHeadLevel();
        }

        private void Update()
        {
            if (!_camera.gameObject.activeSelf) return;

            //Set dragging/panning when we're not hovering over anything
            if ((!EventSystem.current.IsPointerOverGameObject()))
            {
                //Set zoom target
                var scrollDelta = Input.mouseScrollDelta.y;

                if (scrollDelta < 0)
                {
                    zoomTarget = Mathf.Clamp(zoomTarget * 1.2f, 0.05f, 1f);
                }
                else if (scrollDelta > 0)
                {
                    zoomTarget = Mathf.Clamp01(zoomTarget * 0.8f);
                }

                if (scrollDelta != 0) panOffset = Vector3.Lerp(panOffset, Vector3.zero, 0.1f);

                if (Input.GetMouseButtonDown(1))
                {
                    mouseOldPos = Input.mousePosition;
                    dragging = true;
                }

                if (Input.GetMouseButtonDown(2))
                {
                    mouseOldPos = Input.mousePosition;
                    panning = true;
                }
            }

            //Rotation
            if (Input.GetMouseButton(1) && dragging)
            {
                mouseDelta = mouseOldPos - Input.mousePosition;

                cameraRotationTarget.x = cameraRotationTarget.x + mouseDelta.y / 5;
                cameraRotationTarget.y = cameraRotationTarget.y - mouseDelta.x / 5;

                mouseOldPos = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(1))

                dragging = false;

            //Panning
            if (Input.GetMouseButton(2) && panning)
            {
                mouseDelta = mouseOldPos - Input.mousePosition;
                panOffset -= mouseDelta / 500;
                mouseOldPos = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(2))

                panning = false;

            //Reset camera with F
            if (Input.GetKeyDown(KeyCode.F))
            {
                resetCamera();
            }

            //Camera XY
            cameraOffset = Vector3.Lerp(cameraOffsetMin, cameraOffsetMax, zoomTarget) + panOffset;
            //Camera Z offset i.e zoom
            cameraOffset.z = Mathf.Lerp(ZoomMin, ZoomMax, Mathf.Clamp01(zoomTarget));
            //Camera height adjust based on head level
            cameraOffset.y -= headAdjust;

            //Camera offset interp
            _camera.transform.localPosition = Vector3.Lerp(_camera.transform.localPosition, cameraOffset, Time.deltaTime * panSpeed);

            //Rotation interp
            if (rotateSpeed == 0)
            {
                cameraRoot.transform.localRotation = Quaternion.Euler(cameraRotationTarget);
            }
            else
            {
                cameraRoot.transform.localRotation = Quaternion.Slerp(cameraRoot.transform.localRotation, Quaternion.Euler(cameraRotationTarget), Time.deltaTime * rotateSpeed);
            }
        }
    }
}