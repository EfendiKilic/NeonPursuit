using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;

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
    
    [Header("Ayarlar Sekmesi Ses")]
    public AudioMixer gameMixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider masterAudioSlider;

    [Header("Ayarlar Sekmesi Grafik")]
    public TMP_Dropdown qulityDropDown;

    [Header("Ayarlar Sekmesi Dil")]
    public TMP_Dropdown languageDropDown;
    private const string LanguagePreferenceKey = "SelectedLanguage";

    [Header("Ayarlar Sekmesi Tazeleme")]
    public TMP_Dropdown refreshRateDropdown;
    private readonly int[] refreshRates = { 30, 60, 120, 144, 240 }; // Dropdown değerleri
    private const string RefreshRateKey = "MaxRefreshRate"; // PlayerPrefs anahtarı

    #region FPS

    private void PopulateFPSDropdown()
    {
        refreshRateDropdown.options.Clear(); // Mevcut seçenekleri temizle

        foreach (int rate in refreshRates)
        {
            refreshRateDropdown.options.Add(new TMP_Dropdown.OptionData(rate + " FPS"));
        }

        refreshRateDropdown.RefreshShownValue(); // Dropdown'u güncelle
    }

    private void UpdateRefreshRate(int index)
    {
        int selectedRate = refreshRates[index];
        ApplyRefreshRate(selectedRate);

        // Ayarı PlayerPrefs ile kaydet
        PlayerPrefs.SetInt(RefreshRateKey, selectedRate);
        PlayerPrefs.Save();
    }

    private void ApplyRefreshRate(int rate)
    {
        Application.targetFrameRate = rate; // Yeni ekran tazeleme hızını uygula
        Debug.Log($"Ekran tazeleme hızı: {rate} FPS olarak ayarlandı.");
    }
    
    #endregion
    
    #region Dil
    
    private void OnLanguageChanged(int index)
    {
        // Dropdown'dan seçilen Locale Name ile eşleşen dili seç
        var selectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        LocalizationSettings.SelectedLocale = selectedLocale;

        Debug.Log("Dil değiştirildi: " + selectedLocale.LocaleName);
        PlayerPrefs.SetString("dil",selectedLocale.LocaleName);
    }

    
    #endregion

    #region Kalite
    
    public void SetQualityLevelDropdown(int index)
    {
        QualitySettings.SetQualityLevel(index, false);
    }
    
    #endregion

    #region Ses

    
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        gameMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    private void loadMusicVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SetMusicVolume();
    }

    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        gameMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    private void loadSFXVolume()
    {
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        SetMusicVolume();
    }

    public void SetMasterVolume()
    {
        float volume = masterAudioSlider.value;
        gameMixer.SetFloat("master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("masterVolume", volume);
    }

    private void loadMasterVolume()
    {
        masterAudioSlider.value = PlayerPrefs.GetFloat("masterVolume");
        SetMasterVolume();
    }

    #endregion

    private void Start()
    {

        #region FPS

        // Dropdown'a seçenekleri ekle
        PopulateFPSDropdown();

        // Kayıtlı değeri yükle veya varsayılan değeri kullan
        int savedRefreshRate = PlayerPrefs.GetInt(RefreshRateKey, 60);
        int defaultIndex = System.Array.IndexOf(refreshRates, savedRefreshRate);

        if (defaultIndex >= 0 && defaultIndex < refreshRates.Length)
        {
            refreshRateDropdown.value = defaultIndex;
        }

        // Dropdown değiştiğinde metodu bağla
        refreshRateDropdown.onValueChanged.AddListener(UpdateRefreshRate);
        ApplyRefreshRate(savedRefreshRate); // Oyun açıldığında ayarı uygula
        #endregion

        
        #region Dil

        if (PlayerPrefs.HasKey("dil"))
        {
            if (PlayerPrefs.GetString("dil") == "Turkish")
            {
                languageDropDown.value = 0;
                Debug.Log("aaaaaa");
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
            }
            else
            {
                languageDropDown.value = 1;
                Debug.Log("bbbbb");
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
            }
        }
        languageDropDown.onValueChanged.AddListener(OnLanguageChanged); 
        
        #endregion
        
        OpenMainMenu();

        #region Ses

        if (PlayerPrefs.HasKey("musicVolume"))
        {
            loadMusicVolume();
        }
        else
        {
            SetMusicVolume();
        }

        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            loadSFXVolume();
        }
        else
        {
            SetSFXVolume();
        }

        if (PlayerPrefs.HasKey("masterVolume"))
        {
            loadMasterVolume();
        }
        else
        {
            SetMasterVolume();
        }

        #endregion

        #region Çözünürlük

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
        
        #endregion
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
