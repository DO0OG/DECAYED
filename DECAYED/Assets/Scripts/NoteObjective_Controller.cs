using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoteObjective_Controller : MonoBehaviour
{
    public TextMeshProUGUI P_Text;
    public TextMeshProUGUI G_Text;
    public TextMeshProUGUI Obj_Text;

    public Player_Move PM;

    public string G_String;
    public string P_String;
    public string Obj_String;

    // Start is called before the first frame update
    void Start()
    {
        PM = FindObjectOfType<Player_Move>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDisable()
    {
        CancelInvoke();
        PM.Invoke("G_ResetText", 8f);
        PM.Invoke("P_ResetText", 8f);
        Set_G(G_String);
        Set_P(P_String);
        Set_Obj(Obj_String);
    }

    public void Set_G(string text)
    {
        G_Text.text = "∏Ò«• :\n" + text;
    }

    public void Set_P(string text)
    {
        P_Text.text = text;
    }

    public void Set_Obj(string text)
    {
        Obj_Text.text = text;
    }
}
