using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventMonster : MonoBehaviour
{
    bool isTrig = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isTrig)
        {
            transform.position += new Vector3(-0.4f, 0, 0);
        }
    }

    private void OnEnable()
    {
        isTrig = true;
        Invoke(nameof(SetDisable), 2f);
    }

    void SetDisable()
    {
        gameObject.SetActive(false);
        isTrig = false;
    }
}
