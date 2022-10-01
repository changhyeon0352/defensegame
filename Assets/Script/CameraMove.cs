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
    bool isRotating=false;

    private void Awake()
    {
        inputActions = new PlayerInput();
    }
    private void OnEnable()
    {
        inputActions.Camera.Enable();
        inputActions.Camera.CameraMove.performed += OnCameraMove;
        inputActions.Camera.CameraMove.canceled += OnCameraStop;
        inputActions.Camera.CameraRotate.performed += OnCameraRotate;
    }
    private void OnDisable()
    {
        inputActions.Camera.CameraRotate.performed -= OnCameraRotate;
        inputActions.Camera.CameraMove.canceled -= OnCameraStop;
        inputActions.Camera.CameraMove.performed -= OnCameraMove;
        inputActions.Camera.Disable();
        
        
        
    }
    private void OnCameraRotate(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (isRotating)
        {
            Vector2 dirRotate = obj.ReadValue<Vector2>();
            Vector3 eulerAngle = new Vector3(-dirRotate.y, dirRotate.x, 0f);
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
        if (Input.GetMouseButtonDown(1))
        {
            isRotating = true;
        }
        else if(Input.GetMouseButtonUp(1))
        {
            isRotating=false;
        }
        transform.Translate(dir * cameraSpeed * Time.deltaTime);
    }
}
