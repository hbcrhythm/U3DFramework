using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.Mono
{
    public class LookAtCamera : MonoBehaviour
    {
        public bool ignorePitch = false;
        private Transform myTransform;
        private Transform cameraTransform;

        // Use this for initialization
        void Start()
        {
            this.myTransform = this.transform;
            this.cameraTransform = Camera.main ? Camera.main.transform : null;
        }

        // Update is called once per frame
        void Update()
        {
            if (this.cameraTransform)
            {
                Vector3 forward = this.myTransform.position - this.cameraTransform.position;//this.cameraTransform.forward;
                this.myTransform.forward = forward;
                if (this.ignorePitch)
                {
                    Vector3 eulerAngles = this.myTransform.eulerAngles;
                    eulerAngles.x = 0;
                    this.myTransform.eulerAngles = eulerAngles;
                }
            }
            else
            {
                this.cameraTransform = Camera.main ? Camera.main.transform : null;
            }
        }
    }
}