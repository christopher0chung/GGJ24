using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using CDCGameKit;

public class C_NPCSpawner : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI guests;
    [SerializeField] GameObject prefab;
    [SerializeField] float xRange, zRange, yHeight;
    [SerializeField] int countToSpawn;

    [SerializeField] List<B_NPC> npcAgents;

    public LayerMask agentLayerMask;

    private TaskManager tm;
    [SerializeField] int taskCount;
    float newGroupTimer, randomTravelerTimer;
    List<Vector3> travelerDests;

    string[] firstNames = { "Casey", "Sasha", "Jordan", "Charlie", "Kiran", "Sam", "Taylor", "Alex", "Sky", "Riley", "Morgan", "Jamie", "Cameron", "Avery", "Adrian", "Bailey", "Quinn", "Kim", "Robin", "Lee", "Parker", "Peyton", "Phoenix", "Dakota", "Blake", "Leslie", "River", "Skyler", "Hayden", "Emerson", "Francis", "Drew", "Jesse", "Taylor", "Jordan", "Micah", "Logan", "Rowan", "Sidney", "Elliott", "Eden", "Ariel", "Kai", "Pat", "Marley", "Dale", "Devon", "Reese", "Harley" };
    string[] lastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Lee", "White", "Harris", "Martin", "Thompson", "Garcia", "Martinez", "Robinson", "Clark", "Rodriguez", "Lewis", "Lee", "Walker", "Hall", "Allen", "Young", "Hernandez", "King", "Wright", "Lopez", "Hill", "Scott", "Green", "Adams", "Baker", "Gonzalez", "Nelson", "Carter", "Mitchell", "Perez", "Roberts", "Turner", "Phillips", "Campbell", "Parker", "Jackson", "Evans", "Edwards", "Collins", "Stewart", "Sanchez", "Morris", "Rogers", "Reed", "Cook", "Morgan", "Bell", "Murphy", "Bailey", "Rivera", "Cooper", "Richardson", "Cox", "Howard", "Ward", "Torres", "Peterson", "Gray", "Ramirez", "James", "Watson", "Brooks", "Kelly", "Sanders", "Price", "Bennett", "Wood", "Barnes", "Ross", "Henderson", "Coleman", "Jenkins", "Perry", "Powell", "Long", "Patterson", "Hughes", "Flores", "Washington", "Butler", "Simmons", "Foster", "Gonzales", "Bryant", "Alexander", "Russell", "Griffin", "Diaz", "Hayes", "Myers", "Ford", "Hamilton", "Graham", "Sullivan", "Wallace", "Woods", "Chapman", "Duncan", "Armstrong", "Berry", "Fisher", "Curtis", "Stone", "Kennedy", "Willis", "Boyd", "Olson", "Carroll", "Duncan", "Snyder", "Hart", "Cunningham", "Bradley", "Lane" };

    private void Start()
    {
        tm = new TaskManager();
        npcAgents = new List<B_NPC>();
        int firstNameOffset = Random.Range(0, 200);
        int lastNameOffset = Random.Range(0, 200);
        for (int i = 0; i < countToSpawn; i++)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-xRange, xRange), yHeight, Random.Range(-zRange, zRange));
            Quaternion startRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

            GameObject newSpawn = Instantiate(prefab, spawnPos, startRotation);
            newSpawn.name = "PartyGoer" + i.ToString();
            var controller = newSpawn.GetComponent<B_NPC>();
            controller.firstName = firstNames[(i + firstNameOffset) % firstNames.Length] ;
            controller.lastName = lastNames[(i + lastNameOffset) % lastNames.Length] ;
            npcAgents.Add(newSpawn.GetComponent<B_NPC>());
        }
        prefab.SetActive(false);
        _AllTravel();
        _MakeAFarter();
    }

    bool temp;
    public void Update()
    {
        taskCount = tm.tasksRunning;
        tm.Update();

        if (!temp)
        {
            newGroupTimer += Time.deltaTime / 1.3f;
            if (newGroupTimer >= 1)
            {
                newGroupTimer -= 1;
                _MakeGroup();
            }

            randomTravelerTimer += Time.deltaTime / 2f;
            if (randomTravelerTimer >= 1)
            {
                randomTravelerTimer -= 1;
                _RandomTraveler();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            temp = true;
            foreach (var npc in npcAgents)
                npc.agent.isStopped = true;
        }

        if (npcAgents.Count == countToSpawn)
            guests.text = "Guests Present: " + npcAgents.Count;
        else guests.text = "Guests Remaining: " + npcAgents.Count;
    }

    private void _MakeGroup()
    {
        B_NPC convoStarter = npcAgents[Random.Range(0, npcAgents.Count)];

        List<B_NPC> interested = new List<B_NPC>();
        interested.Add(convoStarter);
        
        var found = Physics.OverlapSphere(convoStarter.transform.position, 1, agentLayerMask);
        foreach (var f in found) interested.Add(f.GetComponent<B_NPC>());
        tm.Do(new MakeAConvoGroupTask(interested));
    }

    private void _RandomTraveler()
    {
        if (travelerDests == null)
        {
            travelerDests = new List<Vector3>();
            int sqrt = npcAgents.Count;
            sqrt = (int)Mathf.Sqrt(sqrt);
            sqrt += 2;

            var x = xRange - 1;
            var z = zRange - 1;

            for (int i = 0; i <= sqrt; i++)
            {
                for (int j = 0; j < sqrt; j++)
                {
                    travelerDests.Add(new Vector3(Mathf.Lerp(-x, x, (float)i / sqrt), yHeight, Mathf.Lerp(-z, z, (float)j / sqrt)));
                }
            }
        }

        B_NPC randomTraveler = npcAgents[Random.Range(0, npcAgents.Count)];

        Vector3 newLoc = travelerDests[Random.Range(0, travelerDests.Count)];
        bool goodLoc = false;
        int overflow = 0;

        while (goodLoc == false && overflow < 100)
        {
            var found = Physics.OverlapSphere(newLoc, .5f);
            goodLoc = found.Length == 0;
            overflow++;
            newLoc = travelerDests[Random.Range(0, travelerDests.Count)];
        }

        randomTraveler.agent.SetDestination(newLoc);
        randomTraveler.destinationAssigned = newLoc;
        randomTraveler.agent.isStopped = false;
        randomTraveler.agent.speed = Random.Range(0.5f, 1f);
    }

    private void _AllTravel()
    {
        List<Vector3> locs = new List<Vector3>();
        int sqrt = npcAgents.Count;
        sqrt = (int) Mathf.Sqrt(sqrt);
        sqrt += 2;

        var x = xRange - 1;
        var z = zRange - 1;

        for (int i = 0; i <= sqrt; i++)
        {
            for (int j = 0; j < sqrt; j++)
            {
                locs.Add(new Vector3(Mathf.Lerp(-x, x, (float)i / sqrt), yHeight, Mathf.Lerp(-z, z, (float)j / sqrt)));
            }
        }

        for (int i = 0; i < npcAgents.Count; i++)
        {
            B_NPC traveler = npcAgents[i];

            Vector3 newLoc = locs[Random.Range(0, locs.Count)];
            while (Vector3.Distance(traveler.transform.position, newLoc) < 4)
                newLoc = locs[Random.Range(0, locs.Count)];
            locs.Remove(newLoc);

            traveler.agent.SetDestination(newLoc);
            traveler.destinationAssigned = newLoc;
            traveler.agent.isStopped = false;
            traveler.agent.speed = Random.Range(0.5f, 1f);
        }
    }

    private void _MakeAFarter()
    {
        B_NPC farterSelected = npcAgents[Random.Range(0, npcAgents.Count)];
        farterSelected.isFarter = true;
    }

    public class MakeAConvoGroupTask : Task
    {
        float timer;
        List<B_NPC> interested;
        Vector3 center;
        public MakeAConvoGroupTask(List<B_NPC> group)
        {
            interested = group;
            center = interested[0].transform.position;
        }
        protected override void Initialize()
        {
            var circumfranceRequired = interested.Count * .6f;
            var radiusRequired = circumfranceRequired / (2 * Mathf.PI);

            var angularSeparation = 360 / interested.Count;

            for (int i = 0; i < interested.Count; i++)
            {
                Vector3 destination = center + Quaternion.Euler(0, i * angularSeparation, 0) * Vector3.forward * radiusRequired;
                interested[i].agent.SetDestination(destination);
                interested[i].agent.isStopped = false;
                interested[i].agent.speed = Random.Range(0.5f, 1f);
                interested[i].destinationAssigned = destination;
                interested[i].formationTaskActive = this;
            }
        }

        internal override void Update()
        {
            base.Update();

            if (interested.Count > 0)
            {
                for (int i = interested.Count - 1; i >= 0; i--)
                {
                    bool quitBecauseTaken = interested[i].formationTaskActive != this;

                    if (quitBecauseTaken)
                    {
                        interested.RemoveAt(i);
                        continue;
                    }

                    if (interested[i].agent.isStopped)
                    {
                        var towardsCenter = Quaternion.LookRotation(center - interested[i].transform.position);
                        interested[i].transform.rotation = Quaternion.Slerp(interested[i].transform.rotation, towardsCenter, Time.deltaTime * 4f);
                        //Debug.Log("Turning " + i + " out of " + interested.Count);

                        if (Quaternion.Angle(interested[i].transform.rotation, towardsCenter) < 4)
                        {
                            interested.RemoveAt(i);
                        }
                    }
                    else if (interested[i].distanceToDest < .1f)
                    {
                        //Debug.Log("Set to stop: " + i + " out of " + interested.Count);
                        interested[i].agent.isStopped = true;
                    }
                }
            }
            else
            {
                timer += Time.deltaTime;
                if (timer >= 1) SetStatus(TaskStatus.Success);
            }
        }
    }
}