using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

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
    
    private void Start()
    {
        OpenMainMenu();
    }
    
    private void Update()
    {
        // InputManager'dan ESC veya Gamepad Start kontrolü
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
