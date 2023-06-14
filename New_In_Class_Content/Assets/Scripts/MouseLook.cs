using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [SerializeField] float minViewDistance = 25f;
    [SerializeField] Transform playerBody;
    InputManager controller;

    Vector2 mouselook;

    public float mouseSensitivity = 100f;
    float xRotation = 0f;

    private void Awake()
    {
        controller = new InputManager();
      //  controller.Player.Camera.performed += ctx => Camera(ctx.ReadValue<Vector2>());
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {        
        Camera();
    }

    void Camera()
    {
        mouselook = controller.Player.Camera.ReadValue<Vector2>();
        float mouseX = mouselook.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouselook.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, minViewDistance);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void OnEnable()
    {
        controller.Enable();
    }

    private void OnDisable() 
    {
        controller.Disable();
    } 

}
