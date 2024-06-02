using UnityEngine;
using TMPro;
using System;
using UnityEngine.Rendering;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using Cinemachine;
using Unity.Burst.CompilerServices;

public class GunSystem : MonoBehaviour
{
    //Gun stats
    public float damage, timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    //bools 
    bool shooting, readyToShoot, reloading;

    //Reference
    public CinemachineVirtualCamera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;

    //Graphics
    public GameObject muzzleFlash, bulletHoleGraphic;
    public TextMeshProUGUI text;
    public Sprite weaponIcon;

    //spawn postion
    public Vector3 spawnPos;


    //Audio 
    public AudioSource audio;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip equipSound;
    //Weapon holder animator
    public Animator animator;


    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }
    private void Update()
    {
        MyInput();

        if(text != null)
        {
            //SetText
            text.SetText(bulletsLeft + " / " + magazineSize);
        } else
        {
            Debug.LogWarning("No text object referenced in GunSystem");
        }
        
    }
    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();

        //Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }
    private void Shoot()
    {
        readyToShoot = false;

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);


        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        Debug.DrawRay(fpsCam.transform.position, direction * range, Color.red, 1.0f);

        //Play Audio
        if (audio != null && shootSound != null) audio.PlayOneShot(shootSound);

        //RayCast
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, whatIsEnemy))
        {
            Debug.Log("Raycast hit: " + rayHit.collider.name);

            if (rayHit.collider.gameObject.CompareTag("Enemy"))
            {
                Transform parent = rayHit.collider.transform.parent;

                // Recursively check the parent hierarchy till it reaches the parent object
                while (parent != null)
                {
                    if (parent.CompareTag("Enemy"))
                    {
                        EnemyAnimation enemy = parent.GetComponent<EnemyAnimation>();
                        if (enemy != null)
                        {
                            enemy.TakeDamage(damage);
                            break; // Exit the loop since we found the "Enemy" object
                        }
                    }
                    parent = parent.parent;
                }

            }

            var bulletHoleEffect = Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.Euler(0, 180, 0));
            StartCoroutine(destroyEffect(1f, bulletHoleEffect));
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
        }

        //Graphics
        Quaternion gunRotation = this.transform.rotation;
        var muzzleFlashEffect = Instantiate(muzzleFlash, attackPoint.position, gunRotation); //TODO: Fix position
        muzzleFlashEffect.transform.Rotate(0, -90, 0);
        muzzleFlashEffect.transform.SetParent(this.transform);
        StartCoroutine(destroyEffect(1f, muzzleFlashEffect));

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }
    private void ResetShot()
    {
        readyToShoot = true;
    }
    private void Reload()
    {
        reloading = true;
        animator.SetBool("Reloading", reloading);
        Invoke("ReloadFinished", reloadTime);
        //Play Audio
        if (audio != null && reloadSound != null) audio.PlayOneShot(reloadSound);
    }
    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
        animator.SetBool("Reloading", reloading);
    }

    private IEnumerator destroyEffect(float lifespan, GameObject particle)
    {
        yield return new WaitForSeconds(lifespan);
        Destroy(particle);
    }
}