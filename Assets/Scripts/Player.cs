using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private LayerMask PickableLayerMask;

    [SerializeField]
    private Transform PlayerCameraTransform;

    [SerializeField]
    private GameObject PickUpUI;

    internal void AddHealth(float healthBoost)
    {
        Debug.Log($"Health boosted by {healthBoost}");
    }

    [SerializeField]
    [Min(1)]
    private float HitRange = 3;

    [SerializeField]
    private Transform PickUpParent;

    [SerializeField]
    private GameObject InHandItem;

    [SerializeField]
    private InputActionReference InteractionInput, DropInput, UseInput;

    private RaycastHit Hit;

    [SerializeField]
    private AudioSource PickUpSource;

    private void Start()
    {
        InteractionInput.action.performed += PickUp;
        DropInput.action.performed += Drop;
        UseInput.action.performed += Use;
    }

    private void Use(InputAction.CallbackContext context)
    {
        if (InHandItem != null)
        {
            IUsable usable = InHandItem.GetComponent<IUsable>();
            if (usable != null)
            {
                usable.Use(this.gameObject);
            }
        }
    }

    private void Drop(InputAction.CallbackContext context)
    {
        if (InHandItem != null)
        {
            InHandItem.transform.SetParent(null);
            InHandItem = null;
            Rigidbody rb = Hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }
    }

    private void PickUp(InputAction.CallbackContext context)
    {
        if(Hit.collider != null && InHandItem == null)
        {
            IPickable pickableItem = Hit.collider.GetComponent<IPickable>();
            if (pickableItem != null)
            {
                PickUpSource.Play();
                InHandItem = pickableItem.PickUp();
                InHandItem.transform.SetParent(PickUpParent.transform, pickableItem.KeepWorldPosition);
            }
            //Debug.Log(Hit.collider.name);
            //Rigidbody rb = Hit.collider.GetComponent<Rigidbody>();
            //if (Hit.collider.GetComponent<Food>() || Hit.collider.GetComponent<Weapon>())
            //{
            //    Debug.Log("It's a food!");
            //    InHandItem = Hit.collider.gameObject;
            //    InHandItem.transform.position = Vector3.zero;
            //    InHandItem.transform.rotation = Quaternion.identity;
            //    InHandItem.transform.SetParent(PickUpParent.transform, false);
            //    if (rb != null)
            //    {
            //        rb.isKinematic = true;
            //    }
            //    return;
            //}
            //if (Hit.collider.GetComponent<Item>())
            //{
            //    Debug.Log("It's an item!");
            //    InHandItem = Hit.collider.gameObject;
            //    InHandItem.transform.SetParent(PickUpParent.transform, true);
            //    if (rb != null)
            //    {
            //        rb.isKinematic = true;
            //    }
            //    return;
            //}
        }
        
    }

    private void Update()
    {
        if (Hit.collider != null)
        {
            Hit.collider.GetComponent<Highlight>()?.ToggleHighlight(false);
            PickUpUI.SetActive(false);
        }

        if (InHandItem != null)
        {
            return;
        }

        if (Physics.Raycast(
            PlayerCameraTransform.position, 
            PlayerCameraTransform.forward, out Hit, HitRange,
            PickableLayerMask))
        {
            Hit.collider.GetComponent <Highlight>()?.ToggleHighlight(true);
            PickUpUI.SetActive(true);
        }
    }


}
