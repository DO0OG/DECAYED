using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<Item> Items = new List<Item>();

    public Transform ItemContent;
    public GameObject InventoryItem;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        ItemContent = GameObject.Find("Inventory_Canvas/BG/Inventory/Viewport/Content").transform;
    }

    public void Add(Item item)
    {
        Items.Add(item);
    }

    public void Remove(Item item)
    {
        Items.Remove(item);
    }

    public void ListItems()
    {
        foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in Items)
        {
            GameObject obj = Instantiate(InventoryItem, ItemContent);
            var itemName = obj.transform.Find("Item_Name").GetComponent<Text>();
            var itemIcon = obj.transform.Find("Item_Icon").GetComponent<Image>();
            var itemInfo = obj.transform.Find("Item_Info").GetComponent<TextMeshProUGUI>();

            itemName.text = item.itemName;
            itemIcon.sprite = item.icon;
            itemInfo.text = item.itemInfo;
        }
    }

    public string SerializeItems()
    {
        string itemsJson = JsonUtility.ToJson(new ItemList(Items));
        return itemsJson;
    }

    public void DeserializeItems(string itemsJson)
    {
        ItemList itemList = JsonUtility.FromJson<ItemList>(itemsJson);
        Items = itemList.items;

        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        List<GameObject> selectableObjects = new List<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Selectable"))
            {
                selectableObjects.Add(obj);
            }
        }

        //비교 및 SetActive 설정
        foreach (var obj in selectableObjects)
        {
            bool foundInInventory = Items.Exists(item => item.itemName == obj.name);

            if (!foundInInventory)
            {
                obj.SetActive(true);
            }
            else
            {
                obj.SetActive(false);
            }
        }

        ListItems(); //Items 목록을 업데이트
    }
}

[System.Serializable]
public class ItemList
{
    public List<Item> items;

    public ItemList(List<Item> items)
    {
        this.items = items;
    }
}
