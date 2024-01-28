using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_QuitGameMenu : MonoBehaviour
{
    public GameObject menu;

    private void Start()
    {
        Resume();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menu.activeSelf) Resume();
            else Open();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Resume()
    {
        menu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Open()
    {
        menu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
