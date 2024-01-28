using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CDCGameKit;

public class B_GameOverFrame : MonoBehaviour
{
    bool found;
    public List<GameObject> managedChildren;
    public Image border, frame;
    float timer;

    void Start()
    {
        timer = 0;

        var test = .75f;
        var inverted = test;
        inverted.Invert();

        Debug.Log(test + " " + inverted);
    }

    private void OnEnable()
    {
        EventManager.instance.Register<FarterFound>(Handler);
    }
    private void OnDisable()
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

        }
    }

    Color one, two;
    void Update()
    {
        if (found)
        {

        }
    }
}
