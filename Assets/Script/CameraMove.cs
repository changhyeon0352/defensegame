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
        inputActions.Camera.CameraMove.performed += OnCameraMove;
        inputActions.Camera.CameraMove.canceled += OnCameraStop;
        inputActions.Camera.CameraRotateOnOff.performed += OnCameraRotateOn;
        inputActions.Camera.CameraRotateOnOff.canceled += OnCameraRotateOff;
        inputActions.Camera.CameraRotate.performed += OnCameraRotate;
        inputActions.Camera.CameraZoom.performed += OnCameraZoom;
    }

    

    private void OnDisable()
    {
        inputActions.Camera.CameraZoom.performed -= OnCameraZoom;
        inputActions.Camera.CameraRotate.performed -= OnCameraRotate;
        inputActions.Camera.CameraRotateOnOff.canceled -= OnCameraRotateOff;
        inputActions.Camera.CameraRotateOnOff.performed -= OnCameraRotateOn;
        inputActions.Camera.CameraMove.canceled -= OnCameraStop;
        inputActions.Camera.CameraMove.performed -= OnCameraMove;
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

    private void OnCameraStop(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        dir = Vector2.zero;
    }

    private void OnCameraMove(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        dir = obj.ReadValue<Vector2>().normalized;
    }
    private void Update()
    {
        //transform.Translate(dir * cameraSpeed * Time.deltaTime);
        transform.position += Vector3.right * -dir.y + Vector3.forward * dir.x;
    }
}
