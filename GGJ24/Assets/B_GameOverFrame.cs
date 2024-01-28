using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CDCGameKit;

public class B_GameOverFrame : MonoBehaviour
{
    bool found;
    public List<GameObject> managedChildren;
    public Image border, frame, background;
    float timer;

    void Start()
    {
        timer = 0;

        var test = .75f;
        var inverted = test;
        inverted.Invert();

        Debug.Log(test + " " + inverted);

        foreach (var c in managedChildren)
            c.SetActive(false);
    }

    private void Awake()
    {
        EventManager.instance.Register<FarterFound>(Handler);
    }
    private void OnDestroy()
    {
        EventManager.instance.Unregister<FarterFound>(Handler);
    }
    public void Handler(EventMsg e)
    {
        var fartfound = e as FarterFound;
        if (fartfound != null)
        {
            found = true;
            foreach (var c in managedChildren)
                c.SetActive(true);
            SFX.instance.OneShotUI("PartyHorn");
        }
    }

    Color one, two, three;
    float rate = .25f;
    void Update()
    {
        if (found)
        {
            timer += Time.deltaTime / rate;
            if (timer >= 1)
            {
                timer -= 1;
                one = Tools.Colors.Random();
                two = Tools.Colors.Random();
                three = Tools.Colors.Random(.4f);
            }
            border.color = border.color.MoveTowards(one, Time.deltaTime / rate);
            frame.color = frame.color.MoveTowards(two, Time.deltaTime / rate);
            background.color = background.color.MoveTowards(three, Time.deltaTime / rate);
        }
    }
}
