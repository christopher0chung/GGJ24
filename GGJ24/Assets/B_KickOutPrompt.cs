using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class B_KickOutPrompt : MonoBehaviour
{
    public C_NPCSpawner spawner;
    public TextMeshProUGUI nameField;
    public B_NPC npc;

    private void Awake()
    {
        PromptActive(null);
    }

    public void PromptActive(B_NPC npc)
    {
        if (npc == null)
        {
            this.npc = null;
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            this.npc = npc;
            nameField.text = this.npc.firstName + " " + this.npc.lastName;
        }
    }

    public void Yes()
    {
        npc.KickOut();
        PromptActive(null);
    }
    public void No()
    {
        PromptActive(null);
    }
}
