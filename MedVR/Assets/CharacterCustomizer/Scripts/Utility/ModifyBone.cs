using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Animations;

namespace CC
{
    [DefaultExecutionOrder(50)]
    public class ModifyBone : MonoBehaviour
    {
        public CC_ModifyType Type;

        public bool Inverted = false;

        public float currentValue = 1f;

        [Range(0f, 1f)]
        public float alpha = 1;

        public bool updates;

        private Animator animator;
        public bool ragdolling;

        private Vector3 positionOffset;
        private Vector3 rotationOffset;

        public Transform constraintRoot;
        private Transform constraintObj;
        private PositionConstraint posConstraint;
        private RotationConstraint rotConstraint;

        public bool xAxis;
        public bool yAxis;
        public bool zAxis;

        private void Awake()
        {
            animator = GetComponentInParent<Animator>(true);
        }

        public void Modify()
        {
            switch (Type)
            {
                case CC_ModifyType.ButtSize:
                case CC_ModifyType.HeadSize:
                case CC_ModifyType.Height:
                    transform.localScale = new Vector3(currentValue, currentValue, currentValue);
                    break;

                case CC_ModifyType.TorsoHeight:
                case CC_ModifyType.NeckLength:
                case CC_ModifyType.LegsWidth:
                case CC_ModifyType.ShoulderWidth:
                case CC_ModifyType.HeightOffset:
                    float val = currentValue / 100 * (Inverted ? 1 : -1);
                    getPosConstraint();
                    togglePosConstraint();
                    constraintObj.localPosition = new Vector3(val * (xAxis ? 1 : 0), val * (yAxis ? 1 : 0), val * (zAxis ? 1 : 0));
                    break;

                case CC_ModifyType.BreastSize:
                    transform.localScale = new Vector3(currentValue, currentValue, currentValue);
                    positionOffset = new Vector3(0.02f * currentValue, 0, 0);
                    break;

                case CC_ModifyType.LowerWaistSize:
                case CC_ModifyType.MidWaistSize:
                case CC_ModifyType.UpperWaistSize:
                case CC_ModifyType.UpperTorsoSize:
                case CC_ModifyType.ThighScale:
                case CC_ModifyType.CalfScale:
                case CC_ModifyType.UpperArmScale:
                case CC_ModifyType.LowerArmScale:
                case CC_ModifyType.NeckScale:
                case CC_ModifyType.HipWidth:
                    transform.localScale = new Vector3(1, currentValue, currentValue);
                    break;

                case CC_ModifyType.FootRotation:
                case CC_ModifyType.BallRotation:
                    float rot = currentValue * (Inverted ? -1 : 1);
                    getRotConstraint();
                    toggleRotConstraint();
                    rotationOffset = new Vector3(rot * (xAxis ? 1 : 0), rot * (yAxis ? 1 : 0), rot * (zAxis ? 1 : 0));
                    constraintObj.localEulerAngles = rotationOffset;
                    break;
            }
        }

        private void setUpPosConstraint()
        {
            if (constraintRoot == null) return;

            createConstraintObj();

            //Constraint
            posConstraint = GetComponent<PositionConstraint>();
            if (posConstraint == null) posConstraint = gameObject.AddComponent<PositionConstraint>();

            posConstraint.translationOffset = constraintObj.InverseTransformVector(transform.position - constraintObj.position);

            posConstraint.weight = alpha;
            ConstraintSource constraintSource = new() { sourceTransform = constraintObj, weight = 1.0f };
            if (posConstraint.sourceCount == 0) posConstraint.AddSource(constraintSource);
            else posConstraint.SetSource(0, constraintSource);
            posConstraint.locked = true;
            posConstraint.constraintActive = true;
        }

        private PositionConstraint getPosConstraint()
        {
            if (posConstraint == null) setUpPosConstraint();
            return posConstraint;
        }

        private void setUpRotConstraint()
        {
            if (constraintRoot == null) return;

            createConstraintObj();

            //Constraint
            rotConstraint = GetComponent<RotationConstraint>();
            if (rotConstraint == null) rotConstraint = gameObject.AddComponent<RotationConstraint>();

            rotConstraint.rotationOffset = transform.localEulerAngles;

            rotConstraint.weight = alpha;
            ConstraintSource constraintSource = new() { sourceTransform = constraintObj, weight = 1.0f };
            if (rotConstraint.sourceCount == 0) rotConstraint.AddSource(constraintSource);
            else rotConstraint.SetSource(0, constraintSource);
            rotConstraint.locked = true;
            rotConstraint.constraintActive = true;
        }

        private RotationConstraint getRotConstraint()
        {
            if (rotConstraint == null) setUpRotConstraint();
            return rotConstraint;
        }

        private void createConstraintObj()
        {
            //Constraint object
            if (constraintObj == null)
            {
                constraintObj = constraintRoot.Find(transform.name + "Constraint");
                if (constraintObj == null)
                {
                    constraintObj = new GameObject(transform.name + "Constraint").transform;
                    constraintObj.SetParent(constraintRoot);
                    constraintObj.localPosition = Vector3.zero;
                    constraintObj.localRotation = Quaternion.identity;
                    constraintObj.localScale = Vector3.one;
                }
            }
        }

        private void toggleRotConstraint()
        {
            if (constraintObj == null) return;
            getRotConstraint().enabled = (currentValue != 0 && updates);
        }

        private void togglePosConstraint()
        {
            if (constraintObj == null) return;
            getPosConstraint().enabled = (currentValue != 0 && updates);
        }

        public void onSimulate(bool value)
        {
        }
    }
}