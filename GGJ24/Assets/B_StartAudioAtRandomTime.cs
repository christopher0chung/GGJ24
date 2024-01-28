using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CDCGameKit;
public class B_StartAudioAtRandomTime : MonoBehaviour
{
    public C_NPCSpawner spawner;
    string partyState, lastState;
    AudioSource AS;
    void Start()
    {
        AS = GetComponent<AudioSource>();
        AS.time = Random.Range(0, AS.clip.length - 2);
        AS.Play();

        partyState = lastState = "Party";
    }

    private void Update()
    {
        if (spawner.GuestsRemainingFrac > .55f) partyState = "Party";
        else if (spawner.GuestsRemainingFrac > .35f) partyState = "Hang";
        else if (spawner.GuestsRemainingFrac > .15f) partyState = "Quiet";
        else partyState = "Bummer";

        if (partyState == "Hang" && lastState != "Hang")
        {
            AS.clip = Resources.Load<AudioClip>("SFX/CrowdMurmur");
            AS.time = Random.Range(0, AS.clip.length - 2);
            AS.Play();
        }
        else if (partyState == "Quiet" && lastState != "Quiet")
        {
            AS.volume = .5f;
        }
        else if (partyState == "Bummer" && lastState != "Bummer")
        {
            AS.volume = .2f;
        }

        lastState = partyState;
    }




}
