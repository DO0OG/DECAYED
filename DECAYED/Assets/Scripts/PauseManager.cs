using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Text ������Ʈ�� ����ϱ� ���� ���ӽ����̽�
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [SerializeField] public Canvas PauseCanvas;
    [SerializeField] public Canvas SettingCanvas;
    public TextMeshProUGUI resumeText;  // Resume ��ư�� Text ������Ʈ
    public TextMeshProUGUI settingsText; // Settings ��ư�� Text ������Ʈ
    public TextMeshProUGUI quitText;     // Quit ��ư�� Text ������Ʈ
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
        //���콺�� Resume ��ư ���� �ö� �� �ؽ�Ʈ ���� ����
        resumeText.color = dRed;
    }

    public void OnMouseExitResume()
    {
        //���콺�� Resume ��ư���� ��� �� �ؽ�Ʈ ���� ������� ����
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
