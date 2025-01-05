#region Libraries

using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;

#endregion

public class MainMenuManager : MonoBehaviour
{
    #region Main Paneller
    
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
    
    #endregion

    #region Credits

    [Header("Emeği Geçenler Link")]
    public string link;
    
    #endregion

    #region Settings

    #region Resolution Settings

    [Header("Ayarlar Sekmesi Çözünürlük")]
    public TMP_Dropdown resolutionDropDown;
    public Toggle fullScreenToggle;
    private Resolution[] allResolutions;
    private bool isFullScreen;
    private int selectedResolutionIndex;
    private List<Resolution> selectedResolutionList = new List<Resolution>();
    
    #endregion
    
    #region Audio Settings

    [Header("Ayarlar Sekmesi Ses")]
    public AudioMixer gameMixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider masterAudioSlider;
    public Slider cutsceneAudioSlider;
    
    #endregion

    #region Graphic Settings

    [Header("Ayarlar Sekmesi Grafik")]
    public TMP_Dropdown qulityDropDown;
    
    #endregion

    #region Language Settings

    [Header("Ayarlar Sekmesi Dil")]
    public TMP_Dropdown languageDropDown;
    private const string LanguagePreferenceKey = "SelectedLanguage";
    private bool isInitializing = true;
    
    #endregion

    #region FPS Settings

    [Header("Ayarlar Sekmesi Tazeleme")]
    public TMP_Dropdown refreshRateDropdown;
    private readonly int[] refreshRates = { 30, 60, 120, 144, 240 }; // Dropdown değerleri
    private const string RefreshRateKey = "MaxRefreshRate"; // PlayerPrefs anahtarı
    
    #endregion

    #region V-Sync Settings

    [Header("Ayarlar Sekmesi V-Sync")]
    public Toggle vSyncToggle;
    
    #endregion

    #region CutScene Settings

    [Header("Ayarlar Sekmesi CutScene")]
    public VideoPlayer cutscenePlayer;
    
    #endregion
    
    #endregion

    private void Start() { StartEditing(); }
    private void Update() { UpdateEditing(); }
    

    #region Unity Etkinlik Fonksiyonları

    private void StartEditing()
    {
        OpenMainMenu();

        #region V-Sync
        
        // İlk kez çalıştırıldığında varsayılanı kapalı olarak ayarla
        if (!PlayerPrefs.HasKey("VSyncEnabled"))
        {
            PlayerPrefs.SetInt("VSyncEnabled", 0); // Varsayılan: Kapalı
            PlayerPrefs.Save();
        }

        // Kaydedilmiş V-Sync durumunu yükle
        int savedVSyncState = PlayerPrefs.GetInt("VSyncEnabled");
        SetVSync(savedVSyncState == 1);

        // Toggle'un varsayılan durumunu ayarla
        if (vSyncToggle != null)
        {
            vSyncToggle.isOn = savedVSyncState == 1;
            vSyncToggle.onValueChanged.AddListener(OnVSyncToggleChanged);
        }

        // V-Sync durumunu konsola yazdır
        Debug.Log($"Oyun başlatıldı: V-Sync {(savedVSyncState == 1 ? "açık" : "kapalı")}");

        #endregion

        #region Grafik Kalitesi

        int savedQualityLevel = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        qulityDropDown.value = PlayerPrefs.GetInt("QualityLevel");
        Debug.Log("Dropdown değeri = " + PlayerPrefs.GetInt("QualityLevel"));
        SetQualityLevel(savedQualityLevel);

        // Dropdown'un varsayılan değerini ayarla
        if (qulityDropDown != null)
        {
            qulityDropDown.value = savedQualityLevel;
            qulityDropDown.onValueChanged.AddListener(SetQualityLevelDropdown);
        }
        
        #endregion

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
        
        // Oyuncunun dil tercihini yükle
        LoadLanguagePreference();

        // Dropdown için değişiklik dinleyici ekle
        languageDropDown.onValueChanged.AddListener(OnLanguageChanged);

        // İlk yükleme tamamlandı
        isInitializing = false;
        
        #endregion

        #region Ses

        if (PlayerPrefs.HasKey("musicVolume")) { loadMusicVolume(); }
        else { SetMusicVolume(); }

        if (PlayerPrefs.HasKey("sfxVolume")) { loadSFXVolume(); }
        else { SetSFXVolume(); }

        if (PlayerPrefs.HasKey("masterVolume")) { loadMasterVolume(); }
        else { SetMasterVolume(); }

        if (PlayerPrefs.HasKey("cutsceneVolume")) { loadCutSceneVolume(); }
        else { SetCutSceneVolume(); }

        #endregion

        #region Çözünürlük

        LoadResolutionSettings();
            
        // Tüm çözünürlükleri al
        allResolutions = Screen.resolutions;
        List<string> resolutionStringList = new List<string>();
        string newRes;
        foreach (Resolution res in allResolutions)
        {
            newRes = res.width.ToString() + " - " + res.height.ToString();
            if (!resolutionStringList.Contains(newRes))
            {
                resolutionStringList.Add(newRes);
                selectedResolutionList.Add(res);
            }
        }
        resolutionDropDown.AddOptions(resolutionStringList);

        // Dropdown ve Toggle durumlarını ayarla
        resolutionDropDown.value = selectedResolutionIndex;
        fullScreenToggle.isOn = isFullScreen;

        // Seçilen çözünürlüğü ve tam ekran ayarını uygula
        Screen.SetResolution(
            selectedResolutionList[selectedResolutionIndex].width,
            selectedResolutionList[selectedResolutionIndex].height,
            isFullScreen
        );
        
        #endregion
        
    }

    private void UpdateEditing()
    {
        if (InputManager.instance.EscControlInput)
        {
            HandleMenuClose();
        }

        if (cutscenePlayer.isPlaying)
        {
            Debug.Log("Video oynuyor: Zaman ");
            if (InputManager.instance.AnyKeyPressed && cutscenePlayer.isPlaying)
            {
                SkipVideo();
            }
        }
        else
        {
            Debug.Log("Video şu anda oynatılmıyor.");
        }
    }
    
    #endregion

    #region Ayarlar

    #region Cut Scene Geçiş

    private void SkipVideo()
    {
        cutscenePlayer.time = cutscenePlayer.length; // Videoyu sona atla
        Debug.Log("Video atlandı!");
    }

    #endregion
    
    #region Ses ve Dil
    
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

    public void SetCutSceneVolume()
    {
        float volume = cutsceneAudioSlider.value;
        gameMixer.SetFloat("cutscene", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("cutsceneVolume", volume);
    }

    private void loadCutSceneVolume()
    {
        cutsceneAudioSlider.value = PlayerPrefs.GetFloat("cutsceneVolume");
        SetCutSceneVolume();
    }

    #endregion
    
    #region Dil
    
    private void OnLanguageChanged(int selectedIndex)
    {
        // İlk yükleme sırasında bu işlemi atla
        if (isInitializing) return;

        // Seçilen dili uygula ve kaydet
        if (selectedIndex == 0) // Türkçe
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
            Debug.Log("Dil Türkçe olarak değiştirildi");
            PlayerPrefs.SetString(LanguagePreferenceKey, "Turkish");
        }
        else if (selectedIndex == 1) // İngilizce
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
            Debug.Log("Dil İngilizce olarak değiştirildi");
            PlayerPrefs.SetString(LanguagePreferenceKey, "English");
        }

        PlayerPrefs.Save(); // Kaydı uygula
    }

    private void LoadLanguagePreference()
    {
        // Varsayılan dil İngilizce olarak ayarlanır
        string savedLanguage = PlayerPrefs.GetString(LanguagePreferenceKey, "English");

        if (savedLanguage == "Turkish")
        {
            languageDropDown.value = 0; // Türkçe
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
            Debug.Log("Dil Türkçe olarak yüklendi");
        }
        else
        {
            languageDropDown.value = 1; // İngilizce
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
            Debug.Log("Dil İngilizce olarak yüklendi");
        }
    }
    
    #endregion
    
    #endregion

    #region Kalite FPS ve Ekran

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

    #region Kalite

    public void SetQualityLevelDropdown(int index)
    {
        SetQualityLevel(index);

        // Seçilen kalite seviyesini kaydet
        PlayerPrefs.SetInt("QualityLevel", index);
        PlayerPrefs.Save();
    }

    private void SetQualityLevel(int index)
    {
        QualitySettings.SetQualityLevel(index, false);
    }

    #endregion

    #region V-Sync

    public void OnVSyncToggleChanged(bool isOn)
    {
        SetVSync(isOn);
        Debug.Log($"V-Sync {(isOn ? "açıldı" : "kapandı")}");

        // V-Sync durumunu kaydet
        PlayerPrefs.SetInt("VSyncEnabled", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void SetVSync(bool enabled)
    {
        QualitySettings.vSyncCount = enabled ? 1 : 0;
    }

    #endregion

    #endregion

    #region Çözünürlük ve Tam Ekran Ayarları

    #region Çözünürlük Ayarları
    
    public void ChangeResolution()
    {
        selectedResolutionIndex = resolutionDropDown.value;
        Screen.SetResolution(
            selectedResolutionList[selectedResolutionIndex].width,
            selectedResolutionList[selectedResolutionIndex].height,
            isFullScreen
        );

        // Ayarı kaydet
        SaveSettings();
    }
    
    #endregion

    #region Tam Ekran Ayarlama
    
    public void ChangeFullScreen()
    {
        isFullScreen = fullScreenToggle.isOn;
        Screen.SetResolution(
            selectedResolutionList[selectedResolutionIndex].width,
            selectedResolutionList[selectedResolutionIndex].height,
            isFullScreen
        );

        // Ayarı kaydet
        SaveSettings();
    }
    
    #endregion

    #region Çözünürlük ve Tam Ekran Ayarlarının Kayıtları

    private void SaveSettings()
    {
        // Çözünürlük ve tam ekran ayarlarını kaydet
        PlayerPrefs.SetInt("ResolutionIndex", selectedResolutionIndex);
        PlayerPrefs.SetInt("IsFullScreen", isFullScreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadResolutionSettings()
    {
        // Çözünürlük ve tam ekran ayarlarını yükle
        selectedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0); // Varsayılan 0
        isFullScreen = PlayerPrefs.GetInt("IsFullScreen", 1) == 1;         // Varsayılan true
    }
    
    #endregion
    
    #endregion
    
    #endregion

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
        cutscenePlayer.Play();
        
        cutscenePlayer.loopPointReached += OnVideoEnd;

    }

    void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("Video tamamlandı!");
        // Burada istediğiniz kodu çalıştırabilirsiniz
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

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
