using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class InGameUI : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public Image inventoryFillImage;
    public GameObject pausePanel;
    public GameObject winPanel;
    public GameObject losePanel;
    public Inventory inventory;

    private bool isPaused = false;

    void Start()
    {
        pausePanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        levelText.text = SceneManager.GetActiveScene().name;
    }

    public void OnPauseClicked()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowWinPanel()
    {
        winPanel.SetActive(true);
        //Time.timeScale = 0f;
    }

    public void ShowLosePanel()
    {
        losePanel.SetActive(true);
        //Time.timeScale = 0f;
    }

    public void OnContinueClicked()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnRetryClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnNextLevelClicked()
    {
        Time.timeScale = 1f;
        int currentLevel = int.Parse(SceneManager.GetActiveScene().name.Replace("Level ", ""));
        Debug.Log("Level " + (currentLevel + 1));
        SceneManager.LoadScene("Level " + (currentLevel + 1));
    }
}