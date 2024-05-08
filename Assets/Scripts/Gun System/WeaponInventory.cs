using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class WeaponSwitching : MonoBehaviour
{
    public static WeaponSwitching Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    [Header("References")]
    [SerializeField] private Transform[] weapons;
    [SerializeField] private int maxInventorySize = 4; // Maximum number of weapons in the inventory

    [Header("Keys")]
    [SerializeField] private KeyCode[] keys;

    [Header("Settings")]
    [SerializeField] private float switchTime;

    private int selectedWeapon;
    private float timeSinceLastSwitch;

    private void Start()
    {
        SetWeapons();
        Select(selectedWeapon);

        timeSinceLastSwitch = 0f;
    }

    private void SetWeapons()
    {
        weapons = new Transform[maxInventorySize]; // Initialize the weapons array with the maximum size

        for (int i = 0; i < transform.childCount; i++)
        {
            if (i < maxInventorySize) // Only add children up to the maximum inventory size
            {
                weapons[i] = transform.GetChild(i);
            }
        }

        if (keys == null) keys = new KeyCode[weapons.Length];
    }

    private void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        for (int i = 0; i < keys.Length; i++)
            if (GetActiveWeaponsCount() > i && Input.GetKeyDown(keys[i]) && timeSinceLastSwitch >= switchTime)
                selectedWeapon = i;

        if (previousSelectedWeapon != selectedWeapon) Select(selectedWeapon);

        timeSinceLastSwitch += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropWeapon();
        }
    }

    private void Select(int weaponIndex)
    {
        for (int i = 0; i < weapons.Length; i++)
            weapons[i].gameObject.SetActive(i == weaponIndex);

        timeSinceLastSwitch = 0f;

        OnWeaponSelected();
    }

    private void OnWeaponSelected() { }

    public void PickUpWeapon(GameObject gun)
    {
        if (gun.GetComponent<GunSystem>() != null)
        {

            // Check if the inventory is full
            if (GetActiveWeaponsCount() == maxInventorySize)
            {
                Debug.Log("Inventory is full. Cannot pick up more weapons." + "Inventory size: " + weapons.Length);
                return; // Exit the method early since we cannot add more weapons
            }

            gun.GetComponent<GunSystem>().enabled = true;
            gun.transform.SetParent(transform);

            

            // Set the position and rotation of the gun to the desired position and rotation
            gun.transform.localPosition = gun.GetComponent<GunSystem>().spawnPos;
            gun.transform.localRotation = Quaternion.identity;

            // Add the new weapon to the inventory
            Array.Resize(ref weapons, GetActiveWeaponsCount() + 1);
            weapons[weapons.Length - 1] = gun.transform;

            Array.Resize(ref keys, keys.Length + 1);
            keys[keys.Length - 1] = KeyCode.None;

            EquipWeapon(weapons.Length - 1);
        }
        else
        {
            Debug.Log("Picked up object is not a valid weapon");
        }
    }

    private void EquipWeapon(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < weapons.Length)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                weapons[i].gameObject.SetActive(i == weaponIndex);
            }
            selectedWeapon = weaponIndex;
        }
        else
        {
            Debug.LogError("Invalid weapon index: " + weaponIndex);
        }
    }
    private int GetActiveWeaponsCount()
    {
        int count = 0;
        foreach (Transform weapon in weapons)
        {
            if (weapon != null /*&& weapon.gameObject.activeInHierarchy*/)
            {
                count++;
            }
        }
        return count;
    }
    private void DropWeapon()
        {
            if (selectedWeapon < 0 || selectedWeapon >= weapons.Length)
            {
                Debug.LogError("Invalid weapon index: " + selectedWeapon);
                return;
            }

            // Get the weapon to drop
            Transform weaponToDrop = weapons[selectedWeapon];

            // Remove the weapon from the inventory
            for (int i = selectedWeapon; i < weapons.Length - 1; i++)
            {
                weapons[i] = weapons[i + 1];
            }
            Array.Resize(ref weapons, weapons.Length - 1);

            // Remove the corresponding key
            for (int i = selectedWeapon; i < keys.Length - 1; i++)
            {
                keys[i] = keys[i + 1];
            }
            Array.Resize(ref keys, keys.Length - 1);

            // If the selected weapon was the last in the array, select the new last
            if (selectedWeapon == weapons.Length)
            {
                selectedWeapon--;
            }

            // Drop the weapon
            Drop(weaponToDrop);

            // Select the next weapon in the inventory
            EquipWeapon(selectedWeapon);
        }
    private void Drop(Transform weaponToDrop)
    {
        // Detach the weapon from the parent
        weaponToDrop.SetParent(null);

        // Set the rotation of the weapon
        weaponToDrop.eulerAngles = new Vector3(weaponToDrop.position.x, weaponToDrop.position.z, weaponToDrop.position.y);

        // Enable physics on the weapon
        Rigidbody rb = weaponToDrop.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // Enable the mesh collider on the weapon
        BoxCollider mc = weaponToDrop.GetComponent<BoxCollider>();
        if (mc != null)
        {
            mc.enabled = true;
        }
    }

}
