using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

[Serializable]
public class SaveData
{
    public SerializableVector3 playerPosition;
    public SerializableQuaternion playerRotation;
    public float batteryLife;
    public float currentHealth;
    public SerializableVector3 camPosition;
    public SerializableQuaternion camRotation;
    public SerializableVector3 flashPosition;
    public SerializableQuaternion flashRotation;
    public SerializableVector3 trackerPosition;
    public SerializableQuaternion trackerRotation;
    public string currentSceneName;
}

[Serializable]
public struct SerializableVector3
{
    public float x, y, z;

    public SerializableVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

[Serializable]
public struct SerializableQuaternion
{
    public float x, y, z, w;

    public SerializableQuaternion(Quaternion quaternion)
    {
        x = quaternion.x;
        y = quaternion.y;
        z = quaternion.z;
        w = quaternion.w;
    }

    public Quaternion ToQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }
}

public class SaveLoadManager : MonoBehaviour
{
    private static SaveLoadManager instance;

    [SerializeField]
    private Transform player;
    private Transform cam;
    private Transform tracker;
    private Transform FF;
    private NavMeshAgent navMeshAgent;
    private Player_Move PM;
    private Player_Health PH;
    private InventoryManager IM;
    private CanvasGroup canvasGroup;

    private string saveFilePath;
    private string inventoryFilePath;

    private void Awake()
    {
        //Singleton method
        if (instance == null)
        {
            //First run, set the instance
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            //Instance is not the same as the one we have, destroy old one, and reset to newest one
            Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        saveFilePath = Path.Combine(Application.persistentDataPath, "saveData.dat");
        inventoryFilePath = Path.Combine(Application.persistentDataPath, "inventory.dat");

        if (PlayerPrefs.GetInt("isSave") == 1)
        {
            Save();
            if (SceneManager.GetActiveScene().name == "Chap1")
            {
                PM.G_Text.text = "OBJECTIVE :\nTAKE THE FLASHLIGHT BEHIND THE TRUCK";
                PM.Obj_Text.text = "TAKE THE FLASHLIGHT BEHIND THE TRUCK";
                PM.Invoke("G_ResetText", 10f);
            }
            PlayerPrefs.SetInt("isSave", 0);
        }
    }
    public static SaveLoadManager GetInstance()
    {
        return instance;
    }

    public void Save()
    {
        player = FindObjectOfType<Player_Move>().transform;
        cam = FindObjectOfType<Camera_Controller>().transform;
        tracker = FindObjectOfType<TrackerAI>().transform;
        FF = FindObjectOfType<FlashLight_Follow>().transform;
        navMeshAgent = FindObjectOfType<TrackerAI>().GetComponent<NavMeshAgent>();
        PM = FindObjectOfType<Player_Move>();
        PH = FindObjectOfType<Player_Health>();
        IM = FindObjectOfType<InventoryManager>();
        canvasGroup = GameObject.Find("HUD/Save").GetComponent<CanvasGroup>();

        StartCoroutine(Fade(true));

        SaveData saveData = new SaveData
        {
            currentSceneName = SceneManager.GetActiveScene().name,
            playerPosition = new SerializableVector3(player.position),
            playerRotation = new SerializableQuaternion(player.rotation),
            batteryLife = PM.batteryLife,
            currentHealth = PH.currentHealth,
            camPosition = new SerializableVector3(cam.position),
            camRotation = new SerializableQuaternion(cam.rotation),
            flashPosition = new SerializableVector3(FF.position),
            flashRotation = new SerializableQuaternion(FF.rotation),
            trackerPosition = new SerializableVector3(tracker.position),
            trackerRotation = new SerializableQuaternion(tracker.rotation)
        };

        //각종 데이터를 바이너리 파일로
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, saveData);
        stream.Close();

        //인벤토리 데이터를 바이너리 파일로 Serialize
        string inventoryJson = IM.SerializeItems();
        File.WriteAllText(inventoryFilePath, inventoryJson);

        StartCoroutine(Fade(false));
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
    }

    public void SaveInv()
    {
        IM = FindObjectOfType<InventoryManager>();

        // 기존 저장된 데이터 불러오기
        if (File.Exists(saveFilePath))
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(saveFilePath, FileMode.Open, FileAccess.Read);
            SaveData saveData = (SaveData)formatter.Deserialize(stream);
            stream.Close();

            // 필요한 부분만 업데이트
            Player_Health PH = FindObjectOfType<Player_Health>();
            Player_Move PM = FindObjectOfType<Player_Move>();
            saveData.batteryLife = PM.batteryLife;
            saveData.currentHealth = PH.currentHealth;

            // 업데이트된 데이터를 다시 저장
            stream = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, saveData);
            stream.Close();
        }

        //인벤토리 데이터를 바이너리 파일로 Serialize
        string inventoryJson = IM.SerializeItems();
        File.WriteAllText(inventoryFilePath, inventoryJson);
    }

    public void SaveDead()
    {
        IM = FindObjectOfType<InventoryManager>();

        // 기존 저장된 데이터 불러오기
        if (File.Exists(saveFilePath))
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(saveFilePath, FileMode.Open, FileAccess.Read);
            SaveData saveData = (SaveData)formatter.Deserialize(stream);
            stream.Close();

            // 필요한 부분만 업데이트
            Player_Health PH = FindObjectOfType<Player_Health>();
            Player_Move PM = FindObjectOfType<Player_Move>();
            saveData.batteryLife = PM.batteryLife;
            saveData.currentHealth = PH.maxHP;

            // 업데이트된 데이터를 다시 저장
            stream = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, saveData);
            stream.Close();
        }

        //인벤토리 데이터를 바이너리 파일로 Serialize
        string inventoryJson = IM.SerializeItems();
        File.WriteAllText(inventoryFilePath, inventoryJson);
    }

    public void Load()
    {
        if (File.Exists(saveFilePath))
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(saveFilePath, FileMode.Open, FileAccess.Read);
            SaveData saveData = (SaveData)formatter.Deserialize(stream);
            stream.Close();

            player = FindObjectOfType<Player_Move>().transform;
            cam = FindObjectOfType<Camera_Controller>().transform;
            tracker = FindObjectOfType<TrackerAI>().transform;
            FF = FindObjectOfType<FlashLight_Follow>().transform;
            navMeshAgent = FindObjectOfType<TrackerAI>().GetComponent<NavMeshAgent>();
            PM = FindObjectOfType<Player_Move>();
            PH = FindObjectOfType<Player_Health>();
            IM = FindObjectOfType<InventoryManager>();

            player.position = saveData.playerPosition.ToVector3();
            player.rotation = saveData.playerRotation.ToQuaternion();

            PM.batteryLife = saveData.batteryLife;
            PM.flashBar.maxValue = PM.maxBatteryLife;
            PM.flashBar_Inv.maxValue = PM.maxBatteryLife;

            PH.currentHealth = saveData.currentHealth;

            cam.position = saveData.camPosition.ToVector3();
            cam.rotation = saveData.camRotation.ToQuaternion();

            FF.position = saveData.flashPosition.ToVector3();
            FF.rotation = saveData.flashRotation.ToQuaternion();

            tracker.position = saveData.trackerPosition.ToVector3();
            tracker.rotation = saveData.trackerRotation.ToQuaternion();
            
            if(navMeshAgent != null)
            {
                navMeshAgent.SetDestination(tracker.position);
            }

            //인벤토리 데이터를 Deserialize
            if (File.Exists(inventoryFilePath))
            {
                string inventoryJson = File.ReadAllText(inventoryFilePath);
                IM.DeserializeItems(inventoryJson);
            }

            PM.G_ResetText();
            PM.P_ResetText();
        }
        else if (!File.Exists(saveFilePath))
        {
            //인벤토리 데이터를 Deserialize
            if (File.Exists(inventoryFilePath))
            {
                string inventoryJson = File.ReadAllText(inventoryFilePath);
                IM.DeserializeItems(inventoryJson);
            }

            PM.G_ResetText();
            PM.P_ResetText();
        }
        else
        {
            Debug.LogWarning("Save file not found.");
        }
    }
    public void LoadInv()
    {
        if (File.Exists(saveFilePath))
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(saveFilePath, FileMode.Open, FileAccess.Read);
            SaveData saveData = (SaveData)formatter.Deserialize(stream);
            stream.Close();

            PM = FindObjectOfType<Player_Move>();
            PH = FindObjectOfType<Player_Health>();
            IM = FindObjectOfType<InventoryManager>();

            PM.batteryLife = saveData.batteryLife;
            PM.flashBar.maxValue = PM.maxBatteryLife;
            PM.flashBar_Inv.maxValue = PM.maxBatteryLife;

            PH.currentHealth = saveData.currentHealth;

            //인벤토리 데이터를 Deserialize
            if (File.Exists(inventoryFilePath))
            {
                string inventoryJson = File.ReadAllText(inventoryFilePath);
                IM.DeserializeItems(inventoryJson);
            }

            PM.G_ResetText();
            PM.P_ResetText();
        }
        else if (!File.Exists(saveFilePath))
        {
            //인벤토리 데이터를 Deserialize
            if (File.Exists(inventoryFilePath))
            {
                string inventoryJson = File.ReadAllText(inventoryFilePath);
                IM.DeserializeItems(inventoryJson);
            }

            PM.G_ResetText();
            PM.P_ResetText();
        }
        else
        {
            Debug.LogWarning("Save file not found.");
        }
    }
}