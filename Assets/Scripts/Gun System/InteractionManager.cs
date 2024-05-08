using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; set; }
    public GunSystem hoveredGun = null;
    public KeyCode pickUpKey = KeyCode.E;
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

        if(Physics.Raycast(ray, out hit))
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

            } else
            {
                if(hoveredGun != null)
                {
                    hoveredGun.GetComponent<Outline>().enabled = false;
                }
            }
        }

    }
}
