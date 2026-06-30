using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CC
{
    [DefaultExecutionOrder(200)]
    public class CC_UI_Manager : MonoBehaviour
    {
        public static CC_UI_Manager instance;

        public delegate void OnHover(string partHovered);

        public event OnHover onHover;

        public delegate void OnDrag(string partX, string partY, float deltaX, float deltaY, bool first, bool last);

        public event OnDrag onDrag;

        private bool Dragging;
        private string hoveredPart = "";
        private string partX, partY = "";
        private float multX, multY = 1f;
        public float mouseDeltaScale = 0.01f;
        private Vector3 mousePos;

        private Canvas canvas;

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

        [Tooltip("The parent object of your customizable characters")]
        public GameObject CharacterParent;

        private int currentCharacter;

        public List<AudioClip> UISounds = new List<AudioClip>();

        public void Start()
        {
            SetActiveCharacter(0);
        }

        private void Update()
        {
            bool first = !Dragging && Input.GetMouseButton(0);
            bool last = Dragging && !Input.GetMouseButton(0);

            //Set shape on first drag
            if (first)
            {
                partX = ""; partY = "";
                multX = hoveredPart.Contains("_r") ? -1 : 1; multY = -1f;

                if (hoveredPart.Contains("spine_05")) { partX = "BodyCustomization_ShoulderWidth"; partY = "BodyCustomization_TorsoHeight"; }
                else if (hoveredPart.Contains("spine")) { partX = "BodyCustomization_WaistSize"; partY = ""; }
                else if (hoveredPart.Contains("pelvis")) { partX = "BodyCustomization_HipWidth"; partY = ""; }
                else if (hoveredPart.Contains("lowerarm")) { partX = "BodyCustomization_LowerArmScale"; partY = ""; }
                else if (hoveredPart.Contains("upperarm")) { partX = "BodyCustomization_UpperArmScale"; partY = ""; }
                else if (hoveredPart.Contains("thigh")) { partX = "BodyCustomization_ThighScale"; partY = ""; }
                else if (hoveredPart.Contains("calf")) { partX = "BodyCustomization_CalfScale"; partY = ""; }
                else if (hoveredPart.Contains("head")) { partX = "BodyCustomization_HeadSize"; partY = "BodyCustomization_NeckLength"; }
                else if (hoveredPart.Contains("neck")) { partX = "BodyCustomization_NeckScale"; partY = "BodyCustomization_NeckLength"; }
                else if (hoveredPart.Contains("collider_nose")) { partX = "mod_nose_size"; partY = "mod_nose_height"; multY = 1; }
                else if (hoveredPart.Contains("collider_mouth")) { partX = "mod_mouth_size"; partY = "mod_mouth_height"; multY = 1; }
                else if (hoveredPart.Contains("collider_cheekbones")) { partX = "mod_cheekbone_size"; partY = ""; multX *= -1; }
                else if (hoveredPart.Contains("collider_cheeks")) { partX = "mod_cheeks_size"; partY = ""; multX *= -1; }
                else if (hoveredPart.Contains("collider_jaw")) { partX = "mod_jaw_width"; partY = "mod_jaw_height"; multX *= -1; }
                else if (hoveredPart.Contains("collider_chin")) { partX = ""; partY = "mod_chin_size"; }
                else if (hoveredPart.Contains("collider_eye")) { partX = "mod_eyes_narrow"; partY = "mod_eyes_height"; multY = 1; }
                else if (hoveredPart.Contains("collider_brow")) { partX = ""; partY = "mod_brow_height"; }
            }

            Dragging = Input.GetMouseButton(0);

            if (Dragging && getCanvas() != null)
            {
                Vector3 mouseDelta = (Input.mousePosition - mousePos) * mouseDeltaScale / canvas.scaleFactor;
                onDrag?.Invoke(partX, partY, mouseDelta.x * multX, mouseDelta.y * multY, first, last);
            }
            mousePos = Input.mousePosition;
        }

        private Canvas getCanvas()
        {
            if (canvas != null) return canvas;
            else
            {
                canvas = GetComponentInChildren<Canvas>();
                return canvas;
            }
        }

        private void LateUpdate()
        {
            if (Dragging) return;
            onHover?.Invoke(hoveredPart);

            Physics.SyncTransforms();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit) && !EventSystem.current.IsPointerOverGameObject())
            {
                hoveredPart = hit.collider.name;
            }
            else hoveredPart = "";
        }

        public void playUIAudio(int Index)
        {
            var audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource && UISounds.Count > Index) audioSource.clip = UISounds[Index]; audioSource.Play();
        }

        public void SetActiveCharacter(int i)
        {
            if (CharacterParent == null) return;

            currentCharacter = i;

            for (int j = 0; j < CharacterParent.transform.childCount; j++)
            {
                var character = CharacterParent.transform.GetChild(j).gameObject;

                //Reset active character and disable it
                if (character.activeSelf && i != j)
                {
                    var script = character.GetComponentInChildren<CharacterCustomization>();
                    script.LoadFromPreset(script.CharacterName);
                    character.SetActive(false);
                }
                //Enable selected character (its UI is automatically activated)
                else if (i == j) character.SetActive(true);
            }
        }

        public void characterNext()
        {
            SetActiveCharacter(currentCharacter == CharacterParent.transform.childCount - 1 ? 0 : currentCharacter + 1);
        }

        public void characterPrev()
        {
            SetActiveCharacter(currentCharacter == 0 ? CharacterParent.transform.childCount - 1 : currentCharacter - 1);
        }
    }
}