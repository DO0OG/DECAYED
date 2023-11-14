using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    private static LoadingManager instance;

    public static LoadingManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<LoadingManager>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    instance = Create();
                }
            }
            return instance;
        }
    }

    private static LoadingManager Create()
    {
        return Instantiate(Resources.Load<LoadingManager>("LoadingUI"));
    }

    private void Awake()
    {

        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private Slider loadBar;

    private string loadSceneName;

    private SaveLoadManager SLM;

    public void LoadScene(string sceneName)
    {
        gameObject.SetActive(true);
        loadSceneName = sceneName;
        StartCoroutine(LoadSceneProcess());
    }

    private IEnumerator LoadSceneProcess()
    {
        loadBar.value = 0f;
        yield return StartCoroutine(Fade(true));

        AsyncOperation op = SceneManager.LoadSceneAsync(loadSceneName);

        while (!op.isDone)
        {
            loadBar.value = Mathf.Clamp01(op.progress / 0.9f);
            yield return null;
        }
        OnSceneLoaded();

        yield return StartCoroutine(Fade(false));
    }

    private void OnSceneLoaded()
    {
        if (PlayerPrefs.GetInt("isSave") == 2)
        {
            SLM = FindObjectOfType<SaveLoadManager>();
            SLM.Load();
            PlayerPrefs.SetInt("isSave", 0);
            SLM.Save();
        }
        if (PlayerPrefs.GetInt("isSave") == 3)
        {
            SLM = FindObjectOfType<SaveLoadManager>();
            SLM.LoadInv();
            PlayerPrefs.SetInt("isSave", 0);
            SLM.Save();
        }
    }

    private IEnumerator Fade(bool isFadeIn)
    {
        float timer = 0f;
        float startAlpha = isFadeIn ? 0f : 1f;
        float targetAlpha = isFadeIn ? 1f : 0f;

        while (timer <= 1f)
        {
            yield return null;
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer);
        }

        if (!isFadeIn)
        {
            gameObject.SetActive(false);
        }
    }
}
