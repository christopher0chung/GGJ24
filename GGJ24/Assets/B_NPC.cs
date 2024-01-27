using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using CDCGameKit;

public class B_NPC : MonoBehaviour
{
    public B_NPC speakingToLogic;
    private Transform stt;
    public Transform speakingTo 
    {
        get { return stt; }
        private set
        {
            if (value != stt)
            {
                stt = value;
                if (stt != null) speakingToLogic = stt.GetComponent<B_NPC>();
                else speakingToLogic = null;
            }    
        }
    }
    public Transform middleOfParty;

    public C_NPCSpawner.MakeAConvoGroupTask formationTaskActive;

    public NavMeshAgent agent;

    public Vector3 destinationAssigned;
    public float distanceToDest;
    public float distance { get { return Vector3.Distance(destinationAssigned, transform.position); } }

    public bool isFarter;

    public float timer, nextFartInterval;
    public LayerMask NPCs;

    private bool cts;
    public bool comeToStop
    {
        get
        {
            return cts;
        }
        set
        {
            if (value != cts)
            {
                cts = value;
                if (cts)
                {
                    _FindSomeoneToTalkTo();
                }
                else speakingTo = null;
            }
        }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void FixedUpdate()
    {
        distanceToDest = distance;

        if (isFarter)
        {
            timer += Time.fixedDeltaTime;
            if (timer >= nextFartInterval)
            {
                timer = 0;
                nextFartInterval = Random.Range(10, 15);
                SFX.instance.OneShotSFX("Fart1", transform.position + Vector3.up);
                EventManager.instance.Fire(new FartDetected(transform.position));
            }
        }

        if (gameObject.name == "PartyGoer0") Debug.Log("PartyGoer0 " + (agent.velocity.magnitude < .05f) + " " + agent.velocity.magnitude);
        comeToStop = agent.velocity.magnitude < .03f;
        if (comeToStop)
        {
            speakTimer += Time.fixedDeltaTime;

            if (speakingToLogic != null && speakingToLogic.speakingTo != transform) _FindSomeoneToTalkTo();

            if (speakingTo == null)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(middleOfParty.position - transform.position), Time.deltaTime * 5f);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(speakingTo.position - transform.position), Time.deltaTime * 5f);
            }
        }
    }

    private void OnEnable()
    {
        EventManager.instance.Register<FartDetected>(Handler);
    } 
    private void OnDisable()
    {
        EventManager.instance.Unregister<FartDetected>(Handler);
    }

    public void Handler(EventMsg e)
    {
        var fart = e as FartDetected;
        if (fart == null) return;
        float distance = Vector3.Distance(transform.position, fart.pos);

        bool run = Random.Range(0, distance) < 2f ? true : false;
        bool farterRun = isFarter && (Random.Range(0, 3) == 0 ? true: false);

        if (run || farterRun)
        {
            var offset = Random.insideUnitSphere;
            offset.y = 0;
            offset *= .1f;

            agent.SetDestination((transform.position - fart.pos + offset).normalized * 3 + transform.position);
        }
    }

    float speakTimer;
    public bool WillYouSpeakToMe(Transform t)
    {
        if (speakTimer > 3)
        {
            if (comeToStop)
            {
                speakTimer = 0;
                speakingTo = t;
                return true;
            }
            else return false;
        }
        else return false;
    }

    private void _FindSomeoneToTalkTo()
    {
        var talkTargets = Physics.OverlapSphere(transform.position, 2.3f, NPCs);
        if (gameObject.name == "PartyGoer0")
            Debug.Log("PartyGoer0 Talk targets count = " + talkTargets.Length);
        if (talkTargets.Length > 0)
        {
            bool foundSomeone = false;
            for (int i = 0; i < talkTargets.Length; i++)
            {
                if (talkTargets[i].GetComponent<B_NPC>().WillYouSpeakToMe(transform))
                {
                    speakingTo = talkTargets[i].transform;
                    Debug.Log(gameObject.name + " is now speaking to " + speakingTo.name);
                    foundSomeone = true;
                    return;
                }
            }

            if (foundSomeone == false) speakingTo = middleOfParty;
        }
        else speakingTo = middleOfParty;
    }
}

public class FartDetected : EventMsg
{
    public Vector3 pos { get; set; }
    public FartDetected (Vector3 pos)
    {
        this.pos = pos;
    }
}