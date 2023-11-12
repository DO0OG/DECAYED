using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvItem_Controller : MonoBehaviour
{
    public TextMeshProUGUI Info;

    public TextMeshProUGUI item;

    public Image btnImage;
    private Color originalColor;

    private void Awake()
    {
        Transform infoTransform = GameObject.Find("Inventory_Canvas/BG/Information/Information_Text").transform;
        Info = infoTransform.GetComponent<TextMeshProUGUI>();

        Transform itemInfoTransform = transform.Find("Item_Info");
        item = itemInfoTransform.GetComponent<TextMeshProUGUI>();

        btnImage = GetComponent<Image>();
        originalColor = btnImage.color;
    }

    public void UseItem()
    {
        Info.text = "";
        Info.text = item.text;
    }

    public void selColor()
    {
        btnImage.color = Color.gray;
    }

    public void resetColor()
    {
        btnImage.color = originalColor;
    }
}
