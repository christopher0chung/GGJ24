using System.Linq;
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
    public GameObject exclamationMark;

    //public C_NPCSpawner.MakeAConvoGroupTask formationTaskActive;

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

    float moveAroundTimer, moveAroundInterval;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        moveAroundInterval = Random.Range(1f, 30f);
    }
    private void FixedUpdate()
    {
        if (leaving != exclamationMark.activeSelf) exclamationMark.SetActive(leaving);

        distanceToDest = distance;

        if (leaving)
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

        //if (gameObject.name == "PartyGoer0") Debug.Log("PartyGoer0 " + (agent.velocity.magnitude < .05f) + " " + agent.velocity.magnitude);
        comeToStop = agent.velocity.magnitude < .03f;
        if (comeToStop)
        {
            speakTimer += Time.fixedDeltaTime;

            if (speakingToLogic != null && speakingToLogic.speakingTo != transform) _FindSomeoneToTalkTo();

            if (speakingTo != null) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(speakingTo.position - transform.position), Time.deltaTime * 5f);
        }

        moveAroundTimer += Time.deltaTime;
        if (moveAroundTimer >= moveAroundInterval)
        {
            moveAroundInterval = Random.Range(25f, 70f);
            moveAroundTimer = 0;
            Vector3 newLoc = new Vector3(Random.Range(-9f, 9f), .5f, Random.Range(-9f, 9f));
            _MoveAgent("somewhere else to switch it up", newLoc);
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
            if (isFarter) return;

            float distance = Vector3.Distance(transform.position, fart.pos);

            bool run = Random.Range(0, distance) < 2f ? true : false;
            bool tooClose = distance < 2.5f;

            if (run || isFarter)
            {
                var offset = Random.insideUnitSphere;
                offset.y = 0;
                offset *= .1f;

                if (run)
                    _MoveAgent("away from fart", (transform.position - fart.pos + offset).normalized * 3 + transform.position);
                if (isFarter)
                    _MoveAgent("away from fart for deniability", (transform.position - fart.pos + offset).normalized * 3 + transform.position);

            }
            else if (tooClose)
            {
                if (Random.value < .35f) Leave("because too close to fart");
            }
        }

        var kick = e as KickedOut;
        if (kick != null)
        {
            for (int i = bestFriends.Count - 1; i >= 0; i--)
                if (bestFriends[i] == null) bestFriends.RemoveAt(i);

            if (isFarter) return;
            if (kick.who.isFarter) return;

            if (bestFriends.Contains(kick.who)) Leave("because best friend is leaving");
            else
            {
                var frac = spawner.GuestsRemainingFrac;
                Debug.Log(frac);
                frac = frac.Invert();
                Debug.Log(frac);
                frac *= frac;
                Debug.Log(frac);

                if (Random.value <= frac) Leave("because host is being agro");
            }
        }
    }

    float speakTimer;
    public bool WillYouSpeakToMe(Transform t)
    {
        if (leaving) return false;

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

        var talkTargets = Physics.OverlapSphere(transform.position, 2.3f, NPCs).ToList();
        if (talkTargets.Contains(GetComponent<Collider>())) talkTargets.Remove(GetComponent<Collider>());
        if (talkTargets.Count > 0)
        {
            bool foundSomeone = false;
            for (int i = 0; i < talkTargets.Count; i++)
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
        else _TrySomewhereElse();
    }
    private void _TrySomewhereElse()
    {
        Vector3 newLoc = new Vector3(Random.Range(-9f, 9f), .5f, Random.Range(-9f, 9f));
        _MoveAgent("somewhere else at the party because no one to talk to", newLoc);
    }
    private void _MoveAgent(string whereAndWhy, Vector3 loc)
    {
        Debug.Log(firstName + " " + lastName + " is moving to " + whereAndWhy + ".");
        agent.SetDestination(loc);
    }

    public bool leaving;
    public void KickOut()
    {
        if (isFarter) EventManager.instance.Fire(new FarterFound());
        else EventManager.instance.Fire(new KickedOut(this));
        Leave("because kicked out by host");
    }

    private void Leave(string extraReason = "")
    {
        leaving = true;
        agent.speed += .5f;
        string reason = "leaving the party";
        if (extraReason != "") reason += " " + extraReason;
        _MoveAgent(reason, exit.position);
        spawner.Leaving(this);
        var avatar = transform.GetComponentInChildren<B_RandomConfigurator>();

        if (isFarter) avatar.GetSad();
        else avatar.GetMad();
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

public class FarterFound : EventMsg { }