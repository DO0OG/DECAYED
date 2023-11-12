using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Text 컴포넌트를 사용하기 위한 네임스페이스
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [SerializeField] public Canvas PauseCanvas;
    [SerializeField] public Canvas SettingCanvas;
    public TextMeshProUGUI resumeText;  // Resume 버튼의 Text 컴포넌트
    public TextMeshProUGUI settingsText; // Settings 버튼의 Text 컴포넌트
    public TextMeshProUGUI quitText;     // Quit 버튼의 Text 컴포넌트
    public TextMeshProUGUI backText;

    public AudioSource AudioSource;
    public AudioClip overSound;

    public Color dRed = new Color32(200, 0, 0, 25);

    public Player_Move PM;

    private void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
        PauseCanvas.enabled = false;
        SettingCanvas.enabled = false;
    }

    public void ResumeGame()
    {
        PM.isPause = false;
        PM.isMenu = false;
        PM.isInven = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        PauseCanvas.enabled = false;

        if (PM.isFlash)
        {
            PM.flashBarCanvas.alpha = 1;
        }
    }

    public void Settings()
    {
        PauseCanvas.enabled = false;
        SettingCanvas.enabled = true;
    }

    public void BackToSet()
    {
        PauseCanvas.enabled = true;
        SettingCanvas.enabled = false;
    }

    public void QuitToMain()
    {
        DeactivateAllObjects();
        Time.timeScale = 1;
        Destroy(gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    private void DeactivateAllObjects()
    {
        GameObject[] gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(false);
        }
    }

    public void playSound()
    {
        AudioSource.volume = 0.3f;
        AudioSource.PlayOneShot(overSound);
    }

    public void OnMouseEnterResume()
    {
        //마우스가 Resume 버튼 위로 올라갈 때 텍스트 색상 변경
        resumeText.color = dRed;
    }

    public void OnMouseExitResume()
    {
        //마우스가 Resume 버튼에서 벗어날 때 텍스트 색상 원래대로 변경
        resumeText.color = Color.gray;
    }

    public void OnMouseEnterSettings()
    {
        settingsText.color = dRed;
    }

    public void OnMouseExitSettings()
    {
        settingsText.color = Color.gray;
    }

    public void OnMouseEnterQuit()
    {
        quitText.color = dRed;
    }

    public void OnMouseExitQuit()
    {
        quitText.color = Color.gray;
    }

    public void OnMouseEnterBack()
    {
        backText.color = dRed;
    }

    public void OnMouseExitBack()
    {
        backText.color = Color.gray;
    }
}
