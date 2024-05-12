using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; set; }
    public GunSystem hoveredGun = null;
    public KeyCode pickUpKey = KeyCode.E;
    public float maxPickupDistance = 5f;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        } else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxPickupDistance))
        {
            GameObject objectHitByRay = hit.transform.gameObject;
            if (objectHitByRay.GetComponent<GunSystem>())
            {
                hoveredGun = objectHitByRay.gameObject.GetComponent<GunSystem>();
                hoveredGun.GetComponent<Outline>().enabled = true;
                if (Input.GetKeyDown(pickUpKey))
                {
                    Debug.Log("Hit a gun, pressed key");
                    WeaponSwitching.Instance.PickUpWeapon(objectHitByRay.gameObject);
                }

            }
            else
            {
                if (hoveredGun != null)
                {
                    hoveredGun.GetComponent<Outline>().enabled = false;

                }
            }
        }

    }

    /*private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxPickupDistance))
        {
            GameObject objectHitByRay = hit.transform.gameObject;
            if (objectHitByRay.GetComponent<GunSystem>())
            {
                // If there's a previously hovered gun, disable its outline
                if (hoveredGun != null && hoveredGun != objectHitByRay.GetComponent<GunSystem>())
                {
                    hoveredGun.GetComponent<Outline>().enabled = false;
                }

                // Update the hovered gun and enable its outline
                hoveredGun = objectHitByRay.GetComponent<GunSystem>();
                hoveredGun.GetComponent<Outline>().enabled = true;

                if (Input.GetKeyDown(pickUpKey))
                {
                    Debug.Log("Hit a gun, pressed key");
                    WeaponSwitching.Instance.PickUpWeapon(objectHitByRay.gameObject);
                }
            }
            else
            {
                // If no gun is hovered, disable the outline of the previously hovered gun
                if (hoveredGun != null)
                {
                    hoveredGun.GetComponent<Outline>().enabled = false;
                    hoveredGun = null; // Reset the hovered gun
                }
            }
        }
        else
        {
            // If the raycast doesn't hit anything, disable the outline of the previously hovered gun
            if (hoveredGun != null)
            {
                hoveredGun.GetComponent<Outline>().enabled = false;
                hoveredGun = null; // Reset the hovered gun
            }
        }
    }*/


}

