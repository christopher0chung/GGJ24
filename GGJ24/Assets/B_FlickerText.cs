using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class B_FlickerText : MonoBehaviour
{
    TextMeshProUGUI text;
    float timer, duration;
    bool partyOrFarty;
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();    
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > duration)
        {
            timer -= duration;
            duration = Random.Range(.15f, .5f);
            partyOrFarty = !partyOrFarty;

            text.text = partyOrFarty ? "IT'S A FUNKY PARTY MYSTERY" : "IT'S A FUNKY FARTY MYSTERY";
        }
    }
}
