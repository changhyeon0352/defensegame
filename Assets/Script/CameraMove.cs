using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    PlayerInput inputActions;
    public float cameraSpeed;
    Vector3 dir;
    public float rotateSpeed;
    public float zoomSpeed;
    bool isRotating=false;

    private void Awake()
    {
        inputActions = GameMgr.Instance.inputActions;
    }
    private void OnEnable()
    {
        inputActions.Camera.Enable();
        inputActions.Camera.CameraRotateOnOff.performed += OnCameraRotateOn;
        inputActions.Camera.CameraRotateOnOff.canceled += OnCameraRotateOff;
        inputActions.Camera.CameraRotate.performed += OnCameraRotate;
        inputActions.Camera.CameraZoom.performed += OnCameraZoom;
    }

    

    private void OnDisable()
    {
        inputActions.Camera.Disable();
    }

    private void OnCameraZoom(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            float y = obj.ReadValue<float>();
            transform.position += Vector3.up * -y * zoomSpeed;
        }
            
    }

    private void OnCameraRotateOff(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        isRotating = false;
    }

    private void OnCameraRotateOn(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if(Input.GetKey(KeyCode.LeftShift))
            isRotating = true;
    }
    private void OnCameraRotate(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (isRotating)
        {
            Vector2 dirRotate = obj.ReadValue<Vector2>();
            Vector3 eulerAngle = new Vector3(-dirRotate.y, 0f, 0f);
            Quaternion rot = transform.rotation;
            transform.rotation=rot*Quaternion.Euler(eulerAngle);
        }
    }

    public void CameraStop()
    {
        dir = Vector2.zero;
    }

    public void CameraGetDir(Vector2 dir)
    {
        this.dir = dir;
    }
    private void Update()
    {
        //transform.Translate(dir * cameraSpeed * Time.deltaTime);
        transform.position += (Vector3.right * -dir.y + Vector3.forward * dir.x)*cameraSpeed*Time.deltaTime;
    }
}
