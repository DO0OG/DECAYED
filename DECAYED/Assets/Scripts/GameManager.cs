using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI startText;
    public TextMeshProUGUI easyText;
    public TextMeshProUGUI normalText;
    public TextMeshProUGUI hardText;
    public TextMeshProUGUI continueText;
    public TextMeshProUGUI settingsText;
    public TextMeshProUGUI quitText;
    public TextMeshProUGUI backText;

    public AudioSource AudioSource;
    public AudioClip overSound;

    public Color dRed = new Color32(200, 0, 0, 25);

    public static GameManager instance; // 게임 매니저의 인스턴스를 저장할 정적 변수
    public static GameObject PauseWindow;
    public Canvas Diff;
    public Canvas MainCanvas;
    public Canvas SettingCanvas;

    public InventoryManager IM;

    private string saveFilePath;

    private void Awake()
    {
        Diff.enabled = false;
        SettingCanvas.enabled = false;
        AudioSource = GetComponent<AudioSource>();

        if (instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }

        instance = this;
        PauseWindow = GameObject.Find("Pause_Canvas");
        IM = FindObjectOfType<InventoryManager>();
        DontDestroyOnLoad(gameObject);
    }

    public void SetDiff()
    {
        Diff.enabled = true;

        EventTrigger eventTrigger = Diff.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = Diff.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { Diff.enabled = false; });

        eventTrigger.triggers.Add(entry);
    }

    public void diffEasy()
    {
        PlayerPrefs.SetFloat ("DiffLevel", 1f);
        StartGame();
    }

    public void diffNormal()
    {
        PlayerPrefs.SetFloat("DiffLevel", 1.5f);
        StartGame();
    }

    public void diffHard()
    {
        PlayerPrefs.SetFloat("DiffLevel", 2f);
        StartGame();
    }


    // 게임 시작 버튼을 누를 때 호출되는 함수
    public void StartGame()
    {
        DeactivateAllObjects();

        if(IM.Items != null)
        {
            IM.Items.Clear();
        }

        // 비동기로 씬 로드
        LoadingManager.Instance.LoadScene("Chap1");

        PlayerPrefs.SetInt("isSave", 1);
    }

    // 게임 이어하기 버튼을 누를 때 호출되는 함수
    public void ContinueGame()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "saveData.dat");

        // 파일이 존재하는지 확인
        if (!File.Exists(saveFilePath))
        {
            return; // 파일이 없을 경우 함수 종료
        }

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(saveFilePath, FileMode.Open, FileAccess.Read);
        SaveData saveData = (SaveData)formatter.Deserialize(stream);
        stream.Close();

        DeactivateAllObjects();
        PlayerPrefs.SetInt("isSave", 2);

        // 비동기로 씬 로드
        LoadingManager.Instance.LoadScene(saveData.currentSceneName);
    }

    public void Settings()
    {
        MainCanvas.enabled = false;
        SettingCanvas.enabled = true;
    }

    public void BackToMain()
    {
        MainCanvas.enabled = true;
        SettingCanvas.enabled = false;
    }

    // 종료 버튼을 누를 때 호출되는 함수
    public void QuitGame()
    {
        // 게임을 종료
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void QuitToMain()
    {
        DeactivateAllObjects();
        Time.timeScale = 1;
        Destroy(gameObject); // GameManager 오브젝트 파괴
        SceneManager.LoadScene("MainMenu");
    }

    public void playSound()
    {
        AudioSource.volume = 0.5f;
        AudioSource.PlayOneShot(overSound);
    }

    public void OnMouseEnterStart()
    {
        startText.color = dRed;
    }

    public void OnMouseExitStart()
    {
        startText.color = Color.gray;
    }

    public void OnMouseEnterEasy()
    {
        easyText.color = dRed;
    }

    public void OnMouseExitEasy()
    {
        easyText.color = Color.gray;
    }

    public void OnMouseEnterNormal()
    {
        normalText.color = dRed;
    }

    public void OnMouseExitNormal()
    {
        normalText.color = Color.gray;
    }

    public void OnMouseEnterHard()
    {
        hardText.color = dRed;
    }

    public void OnMouseExitHard()
    {
        hardText.color = Color.gray;
    }

    public void OnMouseEnterContinue()
    {
        continueText.color = dRed;
    }

    public void OnMouseExitContinue()
    {
        continueText.color = Color.gray;
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

    // 모든 게임 오브젝트를 비활성화
    public void DeactivateAllObjects()
    {
        GameObject[] gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(false);
        }
    }
}
