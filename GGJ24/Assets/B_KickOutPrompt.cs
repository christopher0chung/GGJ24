using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class B_KickOutPrompt : MonoBehaviour
{
    public TextMeshProUGUI nameField;
    public B_NPC npc;
    public void PromptActive(B_NPC npc)
    {
        if (npc == null)
        {
            gameObject.SetActive(false);
            this.npc = null;
        }
        else
        {
            this.npc = npc;
            nameField.text = this.npc.firstName + " " + this.npc.lastName;
        }
    }

    public void Yes()
    {

    }
    public void No()
    {
        PromptActive(null);
    }
}
