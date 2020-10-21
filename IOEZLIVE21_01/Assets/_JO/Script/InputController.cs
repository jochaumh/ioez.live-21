using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CPH;

public class InputController : MonoBehaviour {

    public enum InputState
    {
        startDrag,
        endDrag,
        pinch,
        drag
    }

    public delegate void OnPinchEventDelegate(float delta);
    public delegate void OnDragEventDelegate(InputState state, Vector2 pos);
    public delegate void OnInteractEventDelegate();
    public delegate void OnClickEventDelegate(Vector2 pos , RaycastHit hit);


    private static InputController instance;
    public static InputController Instance
    {
        get {

            if (instance != null) return instance;

            GameObject go = new GameObject("InputController");

            instance = go.AddComponent<InputController>();
            DontDestroyOnLoad(go);

            return instance;
        }
    }

    public event OnPinchEventDelegate           OnPinchEvent;
    public event OnDragEventDelegate            OnDragEvent;
    public event OnInteractEventDelegate        OnInteractEvent;
    public event OnClickEventDelegate           OnClickEvent;


    public bool PinchEnabled;
    public bool DragEnabled;
    public bool ClickEnabled;

    public void EnableAllinputs(bool value) {

            PinchEnabled = DragEnabled = ClickEnabled = value;

    }

    private bool drag;
    private bool mouseDrag;

    private float touchStartTime = 0;

    private Vector2 touchStartPos;
    private Vector3 mouseStartPos;


    private void Update(){

        UpdateTouch();
        UpdateMouse();
    }

    #region Update Mouse

    private void UpdateMouse(){

        UpdateMouseInteraction();

        if(DragEnabled)
            UpdateMouseMovement();
        if(ClickEnabled)
            UpdateMouseClick();

    }

    private void UpdateMouseInteraction(){

        if (OnInteractEvent != null && Input.GetButtonUp("Fire1"))
            OnInteractEvent();

    }

    private void UpdateMouseClick(){

        if (Input.GetButtonDown("Fire1"))
        {
            touchStartTime = Time.time;
            mouseStartPos = Input.mousePosition;
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            float dt = Time.time - touchStartTime;
            float d = (Input.mousePosition - mouseStartPos).magnitude;


            if (d < 20 && dt < 0.2f && OnClickEvent != null) {
                RaycastHit hit = new RaycastHit();

                if (Camera.main != null) {

                    Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Physics.Raycast(r, out hit, 300);
                }

                OnClickEvent(new Vector2(Input.mousePosition.x , Input.mousePosition.y), hit);
            }
        }
    }

    private void UpdateMouseMovement(){

        Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        if (Input.GetButtonUp("Fire1") && mouseDrag) {

            mouseDrag = false;
            OnDragEvent(InputState.endDrag, Vector2.zero);
        }

        if (Input.GetButtonDown("Fire1") && !mouseDrag){

            mouseDrag = true;
            OnDragEvent(InputState.startDrag, pos);
        }
  
        if (!mouseDrag) return;
        
        OnDragEvent(InputState.drag, pos);
    }

    #endregion

    #region Update Touch


    private void UpdateTouch(){

        UpdateTouchInteraction();

        
        if(ClickEnabled)
            UpdateTouchClick();

        if(PinchEnabled)
            UpdatePinch();

        if(DragEnabled)
            UpdateTouchMovement();
    }


    private void UpdateTouchClick(){

        if (Input.touchCount != 1)
            return;

        Touch t = Input.touches[0];

        if (t.phase == TouchPhase.Began){
            touchStartTime = Time.time;
            touchStartPos = t.position;
        }
        else if (t.phase == TouchPhase.Ended)
        {
            float dt = Time.time - touchStartTime;
            float d = (t.position - touchStartPos).magnitude;

            if (d < 3 && dt < 0.3f && OnClickEvent != null){



                RaycastHit hit = new RaycastHit();

                if (Camera.main != null) {
                    Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Physics.Raycast(r, out hit, 300);
                }
                

                OnClickEvent(t.position, hit);

            }
        }


    }

    private void UpdateTouchInteraction(){

        if (Input.touchCount == 0)
            return;

        if (OnInteractEvent != null && Input.touches[0].phase == TouchPhase.Began)
            OnInteractEvent();

    }

    private void UpdateTouchMovement(){

        int len = Input.touchCount;

        if(len != 1 && drag){
            drag = false;
            OnDragEvent(InputState.endDrag, Vector2.zero);
            
        }
        if (len !=1 ) return;


        Vector2 pos = Vector3.zero;

        foreach(Touch t in Input.touches){
            pos += t.position;
        }

        pos /= len;

        if(!drag)
        {
            OnDragEvent(InputState.startDrag, pos);
            drag = true;
        }
        else
            OnDragEvent(InputState.drag, pos);
    }


    private void UpdatePinch(){

        if (Input.touchCount != 2) return;

        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        if (OnPinchEvent != null)
            OnPinchEvent(-deltaMagnitudeDiff);

    }

    #endregion


}
