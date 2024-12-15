using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject quitPanel;

    [Header("First Selected Options")]
    [SerializeField] private GameObject mainMenuObject;
    [SerializeField] private GameObject settingsMenuObject;
    [SerializeField] private GameObject creditsMenuObject;
    [SerializeField] private GameObject quitMenuObject;

    [Header("Emeği Geçenler Link")]
    public string link;

    [Header("Ayarlar Sekmesi Çözünürlük")]
    public TMP_Dropdown resolutionDropDown;
    public Toggle fullScreenToggle;
    
    private Resolution[] allResolutions;
    private bool isFullScreen;
    private int SelectedResolutions;
    private List<Resolution> selectedResolutionList = new List<Resolution>();
    
    private void Start()
    {
        OpenMainMenu();

        isFullScreen = true;
        allResolutions = Screen.resolutions;

        List<string> resolutionStringList = new List<string>();
        string newRes;
        foreach (Resolution res in allResolutions)
        {
            newRes = res.width.ToString() + " x " + res.height.ToString();
            if (!resolutionStringList.Contains(newRes))
            {
                resolutionStringList.Add(newRes);
                selectedResolutionList.Add(res);
            }
        }

        resolutionDropDown.AddOptions(resolutionStringList);
    }


    public void ChangeResolution()
    {
        SelectedResolutions = resolutionDropDown.value;
        Screen.SetResolution(selectedResolutionList[SelectedResolutions].width, selectedResolutionList[SelectedResolutions].height, isFullScreen);
    }
    
    public void ChangeFullScreen()
    {
        isFullScreen = fullScreenToggle.isOn;
         Screen.SetResolution(selectedResolutionList[SelectedResolutions].width, selectedResolutionList[SelectedResolutions].height, isFullScreen);
    }
    
    private void Update()
    {
        if (InputManager.instance.EscControlInput)
        {
            HandleMenuClose();
        }
    }

    #region Panel Yönetimi
    
    public void OpenMainMenu()
    {
        EventSystem.current.SetSelectedGameObject(mainMenuObject);
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        quitPanel.SetActive(false);
    }
    
    public void OpenSettingsPanel()
    {
        EventSystem.current.SetSelectedGameObject(settingsMenuObject);
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        creditsPanel.SetActive(false);
        quitPanel.SetActive(false);
    }
    
    public void OpenCreditsPanel()
    {
        EventSystem.current.SetSelectedGameObject(creditsMenuObject);
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(true);
        quitPanel.SetActive(false);
    }
    
    public void OpenQuitPanel()
    {
        EventSystem.current.SetSelectedGameObject(quitMenuObject);
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        quitPanel.SetActive(true);
    }
    
    #endregion

    #region Buton İşlevleri

    #region Play Button
    
    public void OnPlayButtonPressed()
    {
        Debug.Log("Oyun Sahnesi Açılıyor");
        // Oyun sahnesini yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Burada "GameScene" sahne adını uygun şekilde değiştirin.
    }
    
    #endregion

    #region Settings Buttons
    
    public void OnSettingsButtonPressed()
    {
        OpenSettingsPanel();
    }
    
    public void OnSettingsBackButtonPressed()
    {
        OpenMainMenu();
    }
    
    #endregion

    #region Credits Buttons
    
    public void OnCreditsButtonPressed()
    {
        OpenCreditsPanel();
    }
    
    public void OnCreditsBackButtonPressed()
    {
        OpenMainMenu();
    }
    
    public void OnOpenCreditsLinkPressed()
    {
        Application.OpenURL(link);
    }
    
    #endregion
    
    #region Quit Buttons
    
    public void OnQuitButtonPressed()
    {
        OpenQuitPanel();
    }
    public void OnQuitConfrimButtonPressed()
    {
        Debug.Log("Oyun kapatıldı");
        Application.Quit();
    }
    
    public void OnQuitDisconfrimButtonPressed()
    {
        OpenMainMenu();
    }
    
    #endregion
    
    #endregion
    
    #region Özel Kontroller

    // Aktif paneller varsa OpenMainMenu çağırır
    private void HandleMenuClose()
    {
        if (settingsPanel.activeSelf || creditsPanel.activeSelf || quitPanel.activeSelf)
        {
            OpenMainMenu();
        }
    }

    #endregion
}
