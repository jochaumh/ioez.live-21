using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CPH.Cam
{
    public class CameraController : MonoBehaviour {

        [SerializeField]
        private Transform target;
        [SerializeField]
        private float pinchDump = 0.1f;

        [SerializeField]
        private float maxAngle = 90;
        [SerializeField]
        private float minAngle = 0;
        [SerializeField]
        private float rotationFriction = 0.95f;

        public Transform Target{

            get { return target; }

        }

        private Camera cam;
        private float fieldOfView;

        private void Awake() {
            cam = gameObject.GetComponent<Camera>();
            fieldOfView = cam.fieldOfView;
        }

        // Use this for initialization
        void Start(){

			InputController.Instance.EnableAllinputs(true);

            InputController.Instance.OnPinchEvent += Instance_OnPinchEvent;
            InputController.Instance.OnDragEvent += Instance_OnDragEvent;
        }

        private float rotationSpeedX;
        private float rotationSpeedY;
        private Vector2 lastMouse;
        private void Instance_OnDragEvent(InputController.InputState state, Vector2 pos){

            if (state == InputController.InputState.endDrag) return;

            if (state == InputController.InputState.startDrag)
                lastMouse = pos;

            rotationSpeedX += (pos.x - lastMouse.x) * 0.05f;
            rotationSpeedY += -(pos.y - lastMouse.y) * 0.05f;

        


             lastMouse = pos;


        }


        private void Instance_OnPinchEvent(float delta)
        {

            fieldOfView += -delta * 0.1f;
            fieldOfView = Mathf.Clamp(fieldOfView, 16, 70);

        }

        void LateUpdate() {

            /*
            if (AppController.Mode != AppController.ApplicationMode.interactive)
            	return;
			*/

            float angle = transform.localEulerAngles.x;

            if (angle > 180)
                angle -= 360;

            if (angle + rotationSpeedY <= minAngle && rotationSpeedY < 0) rotationSpeedY = 0;
            else if (angle + rotationSpeedY >= maxAngle && rotationSpeedY > 0) rotationSpeedY = 0;

            cam.fieldOfView += (fieldOfView - cam.fieldOfView) * pinchDump;
            transform.RotateAround(target.position, target.up, rotationSpeedX);
            transform.RotateAround(target.position, transform.right, rotationSpeedY);

            rotationSpeedX *= rotationFriction;
            rotationSpeedY *= rotationFriction;

        }

    }

}