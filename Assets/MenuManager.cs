using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject levelSelectPanel;

    public void OnPlayClicked()
    {
        levelSelectPanel.SetActive(true);
    }

    public void OnLevelSelectClose()
    {
        levelSelectPanel.SetActive(false);
    }

    public void OnLevelClicked(int levelNumber)
    {
        SceneManager.LoadScene("Level " + levelNumber);
    }

    public void OnSettingsClicked()
    {
        settingsPanel.SetActive(true);
    }

    public void OnSettingsClose()
    {
        settingsPanel.SetActive(false);
    }
}