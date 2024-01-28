using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_TextureController : MonoBehaviour
{
    public List<Material> faces;

    float timer, interval;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer -= interval;
            interval = Random.Range(.05f, .25f);
            int frame = Random.Range(0, 5);
            int face = Random.Range(0, faces.Count);

            faces[face].SetFloat("_FrameIndex", frame);
        }
    }
}
