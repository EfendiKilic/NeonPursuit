using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;

    [Header("First Selected Options")]
    [SerializeField] private GameObject mainMenuObject;

    private bool isPaused;
    private GameObject lastSelectedButton;
    
    private void Start()
    {
        mainMenu.SetActive(false);
    }

    private void Update()
    {
        if (InputManager.instance.EscControlInput)
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

    #region Canvas Activitions

    private void OpenMainMenu()
    {
        mainMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(mainMenuObject);
    }
    

    private void CloseAllMenus()
    {
        mainMenu.SetActive(false);
    }
    
    #endregion
    
    #region Menu Button Actions
    
    
    public void OnMainMenuPress()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        Debug.Log("Menüye Dönüldü");
    }
    
    public void OnResumePress()
    {
        Unpause();
    }
    
    #endregion
}
