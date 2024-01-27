using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_StartAudioAtRandomTime : MonoBehaviour
{
    AudioSource AS;
    void Start()
    {
        AS = GetComponent<AudioSource>();
        AS.time = Random.Range(0, AS.clip.length - 2);
        AS.Play();
    }
}
