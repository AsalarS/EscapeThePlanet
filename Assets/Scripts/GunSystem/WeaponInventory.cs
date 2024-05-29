using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Inventory UI")]
    [SerializeField] private Image[] inventorySlots; // Array to hold the UI Image components for the inventory slots
    [SerializeField] private RectTransform selector; // Reference to the selector RectTransform

    [SerializeField] private int selectedWeapon;
    [SerializeField] private float timeSinceLastSwitch;


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
            if (weapons[i] != null)
            {
                weapons[i].gameObject.SetActive(i == weaponIndex);
            }

        timeSinceLastSwitch = 0f;

        OnWeaponSelected();

        // Update the selector position based on the selected weapon
        /*UpdateSelectorPosition(weaponIndex);*/
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
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] == null)
                {
                    weapons[i] = gun.transform;
                    break;
                }
            }

            //Physics
            // Enable physics on the weapon
            Rigidbody rb = gun.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            // Enable the mesh collider on the weapon
            BoxCollider boxcol = gun.GetComponent<BoxCollider>();
            if (boxcol != null)
            {
                boxcol.enabled = false;
            }

            try
            {
                EquipWeapon(Array.IndexOf(weapons, gun.transform));
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            // Update the inventory display
            UpdateInventoryDisplay(gun.GetComponent<GunSystem>(), true);

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
                weapons[i]?.gameObject.SetActive(i == weaponIndex);
            }
            selectedWeapon = weaponIndex;
        }
        else if (weapons.Length == 0)
        {
            Debug.Log("No weapons in inventory.");
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
            if (weapon != null)
            {
                count++;
            }
        }
        return count;
    }

    private void DropWeapon()
    {
        // Check if the inventory is empty
        if (weapons.Length == 0)
        {
            Debug.Log("Inventory is empty.");
            return;
        }

        if (selectedWeapon < 0 || selectedWeapon >= weapons.Length)
        {
            Debug.LogError("Invalid weapon index: " + selectedWeapon);
            return;
        }

        // Clear the ammo text
        GunSystem gunSystem = weapons[selectedWeapon].GetComponent<GunSystem>();
        if (gunSystem != null)
        {
            gunSystem.text.text = "";
        }

        // Get the weapon to drop
        Transform weaponToDrop = weapons[selectedWeapon];

        // Remove the weapon from the inventory
        weapons[selectedWeapon] = null;

        // Rearrange the weapons array to remove gaps
        for (int i = selectedWeapon; i < weapons.Length - 1; i++)
        {
            weapons[i] = weapons[i + 1];
            weapons[i + 1] = null;
        }

        // If the selected weapon was the last in the array, select the new last
        if (selectedWeapon == weapons.Length)
        {
            selectedWeapon--;
        }
        Debug.Log($"Attempting to access weapon at index {selectedWeapon} in an array of length {weapons.Length}");

        // Update the inventory display
        UpdateInventoryDisplay(gunSystem, false);
        /*UpdateSelectorPosition(selectedWeapon);*/

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

    public void UpdateInventoryDisplay(GunSystem gunSystem, bool isAdding)
    {
        if (isAdding) // Adding gun to inventory
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].sprite == null)
                {
                    inventorySlots[i].sprite = gunSystem.weaponIcon;
                    inventorySlots[i].enabled = true;
                    break;
                }
            }
        }
        else // Dropping weapon
        {
            // Clear the corresponding slot's sprite
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].sprite == gunSystem.weaponIcon)
                {
                    inventorySlots[i].sprite = null;
                    inventorySlots[i].enabled = false;
                    break;
                }
            }

            // Rearrange the inventory slots to remove gaps
            for (int i = 0; i < inventorySlots.Length - 1; i++)
            {
                if (inventorySlots[i].sprite == null && inventorySlots[i + 1].sprite != null)
                {
                    inventorySlots[i].sprite = inventorySlots[i + 1].sprite;
                    inventorySlots[i].enabled = true;
                    inventorySlots[i + 1].sprite = null;
                    inventorySlots[i + 1].enabled = false;
                }
            }
        }

        // Update the selector position based on the current weapon
        /*UpdateSelectorPosition(selectedWeapon);*/
    }

    private void UpdateSelectorPosition(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < inventorySlots.Length)
        {
            selector.position = inventorySlots[weaponIndex].transform.position;
        }
        else
        {
            Debug.LogError("Invalid weapon index for selector: " + weaponIndex);
        }
    }
}
