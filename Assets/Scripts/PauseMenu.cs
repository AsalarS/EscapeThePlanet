using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject playerUI;

    [SerializeField] private bool disablePlayerUIWhilePaused;
    public static PauseMenu Instance { get; private set; }

    public static bool isPaused { get; private set; }

    [SerializeField] private CanvasGroup menu;

    [SerializeField] private float fadeSpeed;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        isPaused = false;
        menu.gameObject.SetActive(false);
        menu.alpha = 0;

        // Debug log to check if playerUI is assigned
        if (playerUI == null)
        {
            Debug.LogError("playerUI is not assigned in the inspector.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();

        if (isPaused)
        {
            if (!menu.gameObject.activeSelf)
            {
                menu.gameObject.SetActive(true);
                menu.alpha = 0;
            }
            if (menu.alpha < 1) menu.alpha += Time.unscaledDeltaTime * fadeSpeed;

            if (disablePlayerUIWhilePaused && playerUI != null) playerUI.SetActive(false);
        }
        else
        {
            if (menu.alpha > 0) menu.alpha -= Time.unscaledDeltaTime * fadeSpeed;
            if (menu.alpha <= 0) menu.gameObject.SetActive(false);
        }
    }

    public void UnPause()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (disablePlayerUIWhilePaused && playerUI != null) playerUI.SetActive(true);
    }

    public void QuitGame() => Application.Quit();

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (disablePlayerUIWhilePaused && playerUI != null) playerUI.SetActive(false);
        }
        else
        {
            UnPause();
        }
    }
}
