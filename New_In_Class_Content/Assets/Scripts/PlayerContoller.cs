using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerContoller : MonoBehaviour
{
    private CharacterController characterController;

    public float Speed = 5f;
    // Start is called before the first frame update

    PlayerControlls controls;
    Vector2 move;

    private void Awake()
    {
        controls = new PlayerControlls();
        controls.Gameplay.Movement.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Gameplay.Movement.canceled += ctx => move = Vector2.zero;

    }

    void Start()
    {
       // characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
       // Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
       // characterController.Move(move * Time.deltaTime * Speed);

        Vector2 m = new Vector2(move.x, move.y) * Time.deltaTime;
        transform.Translate(m * Speed,Space.World);
        if (move != Vector2.zero)
        {
            Debug.Log(move);
        }
    }
}