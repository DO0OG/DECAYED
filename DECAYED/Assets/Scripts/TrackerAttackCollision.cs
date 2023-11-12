using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerAttackCollision : MonoBehaviour
{
    public Player_Health PH;

    public float diff = 1;

    // Start is called before the first frame update
    void Start()
    {
        diff = PlayerPrefs.GetFloat("DiffLevel");
        PH = FindObjectOfType<Player_Health>();
    }

    private void OnEnable()
    {
        StartCoroutine("AutoDisable");
    }

    public void ResetHit()
    {
        PH.isHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PH.isHit = true;
            PH.DecreaseHealth(50);
            Invoke("ResetHit", 5f);
        }
    }

    private IEnumerator AutoDisable()
    {
        // 0.1초 후에 오브젝트가 사라지도록 한다
        yield return new WaitForSeconds(0.2f);

        gameObject.SetActive(false);
    }
}
