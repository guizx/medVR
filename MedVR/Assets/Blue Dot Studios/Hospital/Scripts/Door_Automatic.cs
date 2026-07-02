using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BDSHospital
{
    public class Door_Automatic : MonoBehaviour
    {
        public Animator doorAnim;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                doorAnim.SetTrigger("Open");

            }

        }
        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                doorAnim.SetTrigger("Close");
            }
        }
    }
}

