using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using CDCGameKit;

public class B_NPC : MonoBehaviour
{
    public C_NPCSpawner spawner;
    public Transform exit;
    public string firstName, lastName;
    public List<B_NPC> bestFriends;

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

        if (kickedOut)
        {
            if (agent.velocity.magnitude < .05f) 
                agent.Move((exit.position - transform.position).normalized * agent.speed * Time.fixedDeltaTime);
            if (Vector3.Distance(transform.position, exit.position) < 1) DestroyImmediate(this.gameObject);
            return;
        }

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
        EventManager.instance.Register<KickedOut>(Handler);
    } 
    private void OnDisable()
    {
        EventManager.instance.Unregister<FartDetected>(Handler);
        EventManager.instance.Unregister<KickedOut>(Handler);
    }

    public void Handler(EventMsg e)
    {
        var fart = e as FartDetected;
        if (fart != null)
        {
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

        var kick = e as KickedOut;
        if (kick != null)
        {
            for (int i = bestFriends.Count - 1; i >= 0; i--)
                if (bestFriends[i] == null) bestFriends.RemoveAt(i);

            if(bestFriends.Contains(kick.who)) Leave();
        }
    }

    float speakTimer;
    public bool WillYouSpeakToMe(Transform t)
    {
        if (speakTimer > 3)
        {
            if (comeToStop && spawner.BeingTracked(this))
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
        if (spawner.BeingTracked(this) == false) return;

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

    public bool kickedOut;
    public void KickOut()
    {
        EventManager.instance.Fire(new KickedOut(this));
        Leave();
    }

    private void Leave()
    {
        kickedOut = true;
        agent.speed += .5f;
        agent.destination = exit.position;
        spawner.Leaving(this);
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

public class KickedOut : EventMsg
{
    public B_NPC who { get; set; }
    public KickedOut(B_NPC who)
    {
        this.who = who;
    }
}