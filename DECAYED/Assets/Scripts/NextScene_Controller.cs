using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextScene_Controller : MonoBehaviour
{
    public NextScene_Controller instance;

    public SaveLoadManager SLM;

    public bool isTrig = false;

    // Start is called before the first frame update
    void Awake()
    {
        isTrig = false;
        SLM = FindObjectOfType<SaveLoadManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(SLM == null)
        {
            SLM = FindObjectOfType<SaveLoadManager>();
        }
        if (isTrig || Input.GetKeyDown(KeyCode.T))
        {
            isTrig = false;

            SLM.Save();

            PlayerPrefs.SetInt("isSave", 3);

            LoadingManager.Instance.LoadScene("Chap2");
        }
     }

    public void OnCollisionEnter(Collision collision)
    {
        isTrig = true;
    }
}
