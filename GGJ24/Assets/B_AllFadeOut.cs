using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CDCGameKit;

public class B_AllFadeOut : MonoBehaviour
{
    public List<Image> images;
    public List<TextMeshProUGUI> texts;

    float timer, durationTillFade = 3, durationOfFade = 2;

    bool fadeDone;

    private void Update()
    {
        if (fadeDone) return;

        timer += Time.deltaTime;

        if (timer >= durationTillFade)
        {
            foreach (var i in images)
                i.color = i.color.MoveTowards(i.color.AlphaZero(), Time.deltaTime / durationOfFade);
            foreach (var t in texts)
                t.color = t.color.MoveTowards(t.color.AlphaZero(), Time.deltaTime / durationOfFade);
        }

        if (timer >= durationOfFade + durationTillFade)
        {
            fadeDone = true;
            foreach (var i in images) i.gameObject.SetActive(false);
            foreach (var t in texts) t.gameObject.SetActive(false);
        }
    }
}
