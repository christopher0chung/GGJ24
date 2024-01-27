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
        MouseShow(false);
        PromptActive(null);
    }

    private void MouseShow(bool show)
    {
        if (show)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    public void PromptActive(B_NPC npc)
    {
        if (npc == null)
        {
            this.npc = null;
            MouseShow(false);
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            this.npc = npc;
            nameField.text = this.npc.firstName + " " + this.npc.lastName;
            MouseShow(true);
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
