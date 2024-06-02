using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public CanvasGroup mainMenu;  // CanvasGroup of the main menu
    public CanvasGroup settingsMenu;  // CanvasGroup of the settings menu
    public CanvasGroup creditsMenu;  // CanvasGroup of the credits menu
    public CanvasGroup logo; // CanvasGroup of the logo
    public GameObject earthPlanet; // GameObject of the earth planet model
    public float fadeDuration = 0.5f;  // Duration of the fade effect
    public AudioMixer mainAudioMixer; // Audio mixer for the volume control

    private Material earthPlanetMaterial; // Material of the earth planet model

    private Resolution[] resolutions; // Array of available screen resolutions
    public TMPro.TMP_Dropdown resolutionDropdown; // Dropdown for the screen resolution

    // Start is called before the first frame update
    void Start()
    {
        // Ensure main menu is visible and other menus are hidden at the start
        mainMenu.alpha = 1;
        mainMenu.interactable = true;
        mainMenu.blocksRaycasts = true;

        settingsMenu.alpha = 0;
        settingsMenu.interactable = false;
        settingsMenu.blocksRaycasts = false;

        creditsMenu.alpha = 0;
        creditsMenu.interactable = false;
        creditsMenu.blocksRaycasts = false;

        logo.alpha = 1;
        logo.interactable = true;
        logo.blocksRaycasts = true;

        //Dropdown for resolution
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                               resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        /*resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Apply saved resolution if exists
        if (PlayerPrefs.HasKey("SelectedResolution"))
        {
            int savedResolutionIndex = PlayerPrefs.GetInt("SelectedResolution");
            SetResolution(savedResolutionIndex);
        }*/
    }

    //set resolution
    public void SetResolution(int resolutionIndex)
    {
        /*Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("SelectedResolution", resolutionIndex); // Save the selected resolution index*/
    }

    //play game
    public void PlayGame()
    {
        // Load the game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    // Function to fade out the main menu and fade in the settings menu
    public void OpenSettings()
    {
        StartCoroutine(FadeOut(mainMenu, () => {
            StartCoroutine(FadeIn(settingsMenu));
        }));
    }

    // Function to fade out the settings menu and fade in the main menu
    public void BackToMainMenuFromSettings()
    {
        StartCoroutine(FadeOut(settingsMenu, () => {
            StartCoroutine(FadeIn(mainMenu));
        }));
    }

    // Function to fade out the credits menu and fade in the main menu
    public void BackToMainMenuFromCredits()
    {
        StartCoroutine(FadeOut(creditsMenu, () => {
            StartCoroutine(FadeIn(mainMenu));
        }));
    }

    // Function to fade out the main menu and fade in the credits menu
    public void OpenCredits()
    {
        StartCoroutine(FadeOut(mainMenu, () => {
            StartCoroutine(FadeIn(creditsMenu));
        }));
    }

    // Function to fade out the logo
    public void HideLogo()
    {
        StartCoroutine(FadeOut(logo));
    }

    // Function to fade in the logo
    public void ShowLogo()
    {
        StartCoroutine(FadeIn(logo));
    }

    // Coroutine to fade out a CanvasGroup
    IEnumerator FadeOut(CanvasGroup canvasGroup, System.Action onFadeComplete = null)
    {
        float startAlpha = canvasGroup.alpha;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        onFadeComplete?.Invoke();
    }

    // Coroutine to fade in a CanvasGroup
    IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        float startAlpha = canvasGroup.alpha;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    // Coroutine to fade out a Material
    IEnumerator FadeOutMaterial(Material material)
    {
        float startAlpha = material.color.a;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            SetMaterialAlpha(material, Mathf.Lerp(startAlpha, 0, t / fadeDuration));
            yield return null;
        }
        SetMaterialAlpha(material, 0);
    }

    // Coroutine to fade in a Material
    IEnumerator FadeInMaterial(Material material)
    {
        float startAlpha = material.color.a;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            SetMaterialAlpha(material, Mathf.Lerp(startAlpha, 1, t / fadeDuration));
            yield return null;
        }
        SetMaterialAlpha(material, 1);
    }

    // Helper function to set the alpha of a material
    void SetMaterialAlpha(Material material, float alpha)
    {
        Color color = material.color;
        color.a = alpha;
        material.color = color;
    }

    //Set Volume
    public void SetVolume(float volume)
    {
        mainAudioMixer.SetFloat("Volume", volume);
    }


    //set fullscreen
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    // Set Mouse Sensitivity
    public void SetMouseSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
    }

}
