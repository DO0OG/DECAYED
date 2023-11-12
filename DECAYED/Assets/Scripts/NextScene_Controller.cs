using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextScene_Controller : MonoBehaviour
{
    public NextScene_Controller instance;

    public SaveLoadManager SLM;

    // Start is called before the first frame update
    void Start()
    {
        SLM = FindObjectOfType<SaveLoadManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            PlayerPrefs.SetInt("isSave", 2);

            LoadingManager.Instance.LoadScene("Chap2");

            PlayerPrefs.SetInt("isSave", 1);
        }

     }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerPrefs.SetInt("isSave", 2);

        LoadingManager.Instance.LoadScene("Chap2");

        PlayerPrefs.SetInt("isSave", 1);
    }
}
