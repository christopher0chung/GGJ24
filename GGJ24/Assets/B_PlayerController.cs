using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class B_PlayerController : MonoBehaviour
{
    NavMeshAgent agent;
    Transform cam;
    float x, y;

    public GameObject background;
    public TextMeshProUGUI text;

    public List<Image> cursors;

    public B_KickOutPrompt kickoutPrompt;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        cam = transform.GetChild(0);
    }

    private void Update()
    {
        if (kickoutPrompt.gameObject.activeSelf) return;


        x -= Input.GetAxis("Mouse Y") * Time.deltaTime * 180;
        x = Mathf.Clamp(x, -70, 70f);
        y += Input.GetAxis("Mouse X") * Time.deltaTime * 180;
        y %= 360;
        y += 360;
        y %= 360;

        transform.rotation = Quaternion.Euler(0, y, 0);
        cam.transform.localRotation = Quaternion.Euler(x, 0, 0);

        Vector3 moveDir = Vector3.zero;

        moveDir += Input.GetKey(KeyCode.W) ? Vector3.forward : Vector3.zero;
        moveDir += Input.GetKey(KeyCode.S) ? Vector3.back : Vector3.zero;
        moveDir += Input.GetKey(KeyCode.A) ? Vector3.left : Vector3.zero;
        moveDir += Input.GetKey(KeyCode.D) ? Vector3.right : Vector3.zero;

        Vector3 facingDir = transform.forward;

        agent.Move(Quaternion.LookRotation(facingDir) * moveDir * Time.deltaTime * 1.3f);
        
        _ShowPersonName();
    }

    public LayerMask mask;
    RaycastHit hit;
    private void _ShowPersonName()
    {
        bool npcInCrosshair = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, mask, QueryTriggerInteraction.Ignore);
        B_NPC personData = null;

        background.gameObject.SetActive(npcInCrosshair);
        if (npcInCrosshair) 
        { 
            personData = hit.transform.GetComponent<B_NPC>();
            text.text = personData.firstName + " " + personData.lastName;
        }

        bool closeEnough = npcInCrosshair && hit.distance < 2;

        foreach (var c in cursors) c.color = closeEnough ? Color.red : new Color(.2f, .2f, .2f, .5f);

        if (closeEnough)
        {
            if (Input.GetMouseButtonDown(0))
            {
                kickoutPrompt.PromptActive(personData);
            }
        }
    }
}
