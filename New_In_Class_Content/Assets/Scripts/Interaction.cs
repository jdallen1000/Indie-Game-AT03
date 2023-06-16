using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    public Image crosshair;
    [SerializeField] private float reach;
    bool debug;
    InputManager controller;

    // Start is called before the first frame update
    void Start()
    { 
        controller = new InputManager();
    }

    // Update is called once per frame
    public bool active;

    public GameObject interactable;

    // Start is called before the first frame update
    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit rayHit, reach) == true)        //if raycast hits object
        {
            if (rayHit.collider.TryGetComponent(out InteractableObject interactable) == true)                   //if object hit has interactable component
            {
                if (crosshair != null && crosshair.color != Color.green)
                {
                    crosshair.color = Color.green;                  //set crosshair colour green
                }

                if (Input.GetButtonDown("Fire1") == true)           //if interaction input is pressed
                {
                    if (interactable.active == true)           //if item cannot be activated, it must already be active
                    {
                        interactable.active = false;                  //try deactivate the object instead
                    }

                    if (debug == true)
                    {
                        Debug.DrawRay(transform.position, transform.forward * reach, Color.green, 0.25f);       //draw debug ray
                    }
                }
            }
            else if (rayHit.collider.TryGetComponent(out EnemyPathfinding enemyInteract) == true)                   //if object hit has interactable component
            {
                if (crosshair != null && crosshair.color != Color.green)
                {
                    crosshair.color = Color.green;                  //set crosshair colour green
                }

                if (Input.GetButtonDown("Fire1") == true)           //if interaction input is pressed
                {
                    if (enemyInteract.hitStunned == false)
                    {
                        enemyInteract.hitStunned = true;
                    }
                }
            }

            else
            {
                if (crosshair != null && crosshair.color != Color.red)
                {
                    crosshair.color = Color.red;
                }
            }
        }
        else
        {
            crosshair.color = Color.red;
        }
    }
}

