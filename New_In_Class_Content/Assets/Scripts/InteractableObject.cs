using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public bool active;

    public GameObject interactable;
    public bool gotObject = false;

    // Start is called before the first frame update
    void Start()
    {
        active = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            interactable.SetActive(true);
        }
        else
        {
            interactable.SetActive(false);
            gotObject = true;
        }
    }
}
