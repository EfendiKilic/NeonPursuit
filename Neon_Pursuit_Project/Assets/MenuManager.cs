using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject quitMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject creditsMenu;

    [Header("First Selected Options")]
    [SerializeField] private GameObject mainMenuObject;
    [SerializeField] private GameObject settingsMenuObject;
    [SerializeField] private GameObject creditsMenuObject;
    [SerializeField] private GameObject quitMenuObject;
    
    private bool isPaused;
    
    private void Start()
    {
        mainMenu.SetActive(false);
        quitMenu.SetActive(false);
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }

    private void Update()
    {
        if (InputManager.instance.MenuOpenClosetInput)
        {
            if (!isPaused)
            {
                Pause();
            }
            else
            {
                Unpause();
            }
        }
    }

    #region Pause Unpause

    private void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        OpenMainMenu();
    }

    private void Unpause()
    {
        isPaused = false;
        Time.timeScale = 1f;
        CloseAllMenus();
    }
    
    #endregion

    #region Canvas Actions

    private void OpenMainMenu()
    {
        mainMenu.SetActive(true);
        quitMenu.SetActive(false);
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(mainMenuObject);
    }
    
    private void OpenSettingsMenuHandle()
    {
        mainMenu.SetActive(false);
        quitMenu.SetActive(false);
        settingsMenu.SetActive(true);
        creditsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(settingsMenuObject);
    }
    private void OpenCreditsMenuHandle()
    {
        mainMenu.SetActive(false);
        quitMenu.SetActive(false);
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(creditsMenuObject);
    }
    private void OpenQuitMenuHandle()
    {
        mainMenu.SetActive(false);
        quitMenu.SetActive(true);
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(quitMenuObject);
    }
    private void OpenGameScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        mainMenu.SetActive(true);
        quitMenu.SetActive(false);
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }
    private void CloseAllMenus()
    {
        mainMenu.SetActive(false);
        quitMenu.SetActive(false);
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }
    
    #endregion

    #region Menu Button Actions

    public void PlayPress()
    {
        OpenGameScene();
    }

    public void QuitPress()
    {
        OpenQuitMenuHandle();
    }

    public void OnQuitBackPress()
    {
        OpenMainMenu();
    }
    public void OnQuitConfrimPress()
    {
        Application.Quit();
        Debug.Log("Oyun Kapandı");
    }

    public void SettingsPress()
    {
        OpenSettingsMenuHandle();
    }

    public void OnSettingsBackPress()
    {
        OpenMainMenu();
    }

    public void CreditsPress()
    {
        OpenCreditsMenuHandle();
    }

    public void OnCreditsBackPress()
    {
        OpenMainMenu();
    }
    
    public void OnResumePress()
    {
        Unpause();
    }

    #endregion
}