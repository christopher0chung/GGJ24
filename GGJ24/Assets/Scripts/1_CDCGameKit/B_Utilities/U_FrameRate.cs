using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class U_FrameRate : MonoBehaviour
{
    TextMeshProUGUI textMesh;
    float avg, fps, textUpdateTimer;

    public void Start()
    {
        if (textMesh == null) textMesh = GetComponent<TextMeshProUGUI>();

        if (textMesh == null) Debug.LogError(this.GetType().ToString() + " ERROR - missing TextMeshProUGUI on " + gameObject.name);

        textUpdateTimer = .5f;
        textMesh.text = "XXX.XX FPS";
    }

    public void Update()
    {
        avg = Mathf.Lerp(Time.deltaTime, avg, .95f);
        fps = 1 / avg;

        textUpdateTimer += Time.deltaTime;
        if (textUpdateTimer >= .5f)
        {
            textUpdateTimer %= .5f;
            textMesh.text = fps.ToString("###.##") + " FPS";
        }
    }
}
