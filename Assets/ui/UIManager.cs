using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject ux;
    [SerializeField] GameObject exitUI;
    [SerializeField] GameObject deathUI;
    [SerializeField] GameObject settingsUI;

    [SerializeField] UI currentUI = UI.ux;
    public enum UI
    {
        ux,
        exitUI,
        deathUI,
        settingsUI
    }

    [SerializeField] int MainManuNum;


    // Start is called before the first frame update
    void Start()
    {
        ChangeActiveUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentUI == UI.exitUI)
            {
                currentUI = UI.ux;
                ChangeActiveUI();
            }
            else
            {
                currentUI = UI.exitUI;
                ChangeActiveUI();
            }
        }
    }

    public void ChangeToUX()
    {
        currentUI = UI.ux;
        ChangeActiveUI();
    }
    public void ChangeToDeath()
    {
        currentUI = UI.deathUI;
        ChangeActiveUI();
    }
    public void ChangeToSettings()
    {
        currentUI = UI.settingsUI;
        ChangeActiveUI();
    }

    // Call this function to Change to the current active ui
    public void ChangeActiveUI()
    {
        ux.SetActive(false);
        exitUI.SetActive(false);
        deathUI.SetActive(false);
        settingsUI.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (currentUI == UI.ux)
        {
            ux.SetActive(true);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (currentUI == UI.exitUI)
        {
            exitUI.SetActive(true);
            Time.timeScale = 0f;
        }
        if (currentUI == UI.deathUI)
        {
            deathUI.SetActive(true);
            Time.timeScale = 0f;
        }
        if (currentUI == UI.settingsUI)
        {
            settingsUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    // Call this function to restart the current level
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f; // Resume the game
    }

    // Call this function to exit the game
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
    }

    // Call this function to restart the current level
    public void GoToMainManu()
    {
        SceneManager.LoadScene(MainManuNum);
        Time.timeScale = 1f; // Resume the game
    }
}
