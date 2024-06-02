using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private float health;
    private float lerpTimer;
    public float maxHealth = 100f;
    public float chipSpeed = 2f;
    public float currentXP = 0f;
    public float maxXP = 1000f;
    public int currentLvl = 1;
    [Header("Health UI")]
    public Image frontHealthBar;
    public Image backHealthBar;
    public GameObject bloodyScreen;
    private int maxLvl = 5 ;
    private float xpLerpTimer, xpDelayTimer;
    [Header("XP UI")]
    public Image frontXPBar;
    public Image backXPBar;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        frontXPBar.fillAmount = currentXP / maxXP;
        backXPBar.fillAmount = currentXP / maxXP;
    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health,0,maxHealth);
        UpdateHealthUI();
        UpdateXPUI();
        if (Input.GetKeyDown(KeyCode.B))
        {
            TakeDamage(Random.Range(5, 10));
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            RestoreHealth(Random.Range(5, 10));
        }
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            IncreaseXP(20);
        }
    }
    public void UpdateHealthUI()
    {
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;
        if(fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }

        if(fillF < hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
        StartCoroutine(BloodyScreenEffect());
        if(health <= 0f)
        {
            Debug.Log("player dead");
            IsDead();
            playerDead();
        }

    }

    private IEnumerator BloodyScreenEffect()
    {
        if(bloodyScreen.activeInHierarchy == false)
        {
            bloodyScreen.SetActive(true);
        }

        var image = bloodyScreen.GetComponentInChildren<Image>();

        // Set the initial alpha value to 1 (fully visible).
        Color startColor = image.color;
        startColor.a = 1f;
        image.color = startColor;

        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Calculate the new alpha value using Lerp.
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);

            // Update the color with the new alpha value.
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;

            // Increment the elapsed time.
            elapsedTime += Time.deltaTime;

            yield return null; ; // Wait for the next frame.
        }

        if (bloodyScreen.activeInHierarchy)
        {
            bloodyScreen.SetActive(false);
        }

        
    }

    public bool IsDead()
    {
        return health <= 0f;
    }
    public void playerDead()
    {
        PlayerMovment playerMovement = GetComponent<PlayerMovment>();

        // Disable player movement
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Disable mouse look on the main camera
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            MouseLook mouseLook = mainCamera.GetComponent<MouseLook>();
            if (mouseLook != null)
                mouseLook.enabled = false;
        }
        Animator animator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        animator.SetBool("IsDead", true);
        GetComponent<BlackOut>().StartFade();
        
    }


    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;

    }
    private void OnEnable()
    {
        ExperienceManager.Instace.OnExperienceChange += HandleExperienceChange;
    }
    private void OnDisable()
    {
        ExperienceManager.Instace.OnExperienceChange -= HandleExperienceChange;
        
    }
    private void HandleExperienceChange(int newXP)
    {
        IncreaseXP(newXP);
    }
    private void IncreaseXP(int newXP)
    {
        if (currentLvl < maxLvl)
        {
            currentXP += newXP;
            xpLerpTimer = 0f;
            xpDelayTimer = 0f;
            if (currentXP > maxXP)
            {
                levelUp();
            }
        }
    }
    private void levelUp()
    {
        maxHealth += 20f;
        health = maxHealth;
        currentLvl++;
        currentXP = 0;
        maxXP += 250;
        frontXPBar.fillAmount = 0f;
        backXPBar.fillAmount = 0f;
    }

    public void UpdateXPUI()
    {
        float xpFraction = currentXP / maxXP;
        float FXP = frontXPBar.fillAmount;
        if (FXP < xpFraction)
        {
            xpDelayTimer += Time.deltaTime;
            backXPBar.fillAmount = xpFraction;
            if(xpDelayTimer > 3)
            {
                xpLerpTimer += Time.deltaTime;
                float percentComplete = xpLerpTimer / 4;
                frontXPBar.fillAmount = Mathf.Lerp(FXP, backXPBar.fillAmount, percentComplete);
            }
        }
    }
}
