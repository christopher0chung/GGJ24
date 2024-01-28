using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using CDCGameKit;

public class B_RandomConfigurator : MonoBehaviour
{
    [SerializeField] GameObject head, hair, torso, shoulderL, shoulderR, uarmL, uarmR, farmL, farmR, pelvis, ulegL, ulegR, llegL, llegR, footL, footR;

    public List<Material> hairMats, clothingMats, skinMats;

    Animator anim;
    NavMeshAgent agent;


    private void Awake()
    {
        anim = transform.GetComponentInChildren<Animator>();
        agent = transform.parent.GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        DeleteSizeRef();
        PickHead();
        PickHair();
        PickToro();
        PlaceArms();
        HeadToTorso();
        GetRemainingRefs();
        ApplyMats();
    }

    public void Update()
    {
        UArmsUpdate();
        FArmsUpdate();
        HeadBopUdate();

        bool standing = agent.velocity.magnitude < .04f;
        anim.SetBool("Standing", standing);

        if (standing)
        {
            walkCycle = 0;
        }
        else
        {
            walkCycle += Time.deltaTime * agent.velocity.magnitude;
            walkCycle %= 1;
            anim.SetFloat("WalkCycle", walkCycle);
        }
    }

    #region Initializiation Functions
    void DeleteSizeRef()
    {
        var toDelete = transform.Find("Size Guide");
        GameObject.Destroy(toDelete.gameObject);
    }

    void PickHead()
    {
        var children = transform.GetChildrenDeep();

        var heads = new List<Transform>();
        foreach (var c in children)
            if (c.name.Contains("Head")) heads.Add(c);

        //foreach (var h in heads) print(h.name);

        int headIndex = Random.Range(0, heads.Count);
        for (int i = heads.Count - 1; i >= 0; i--)
        {
            if (i != headIndex) GameObject.Destroy(heads[i].gameObject);
            else head = heads[i].gameObject;
        }
    }

    void PickHair()
    {
        var hairs = head.transform.FindAllRecursive("Hair", true);

        var hairIndex = Random.Range(0, hairs.Count);

        for (int i = hairs.Count - 1; i >= 0; i--)
        {
            if (i != hairIndex) GameObject.Destroy(hairs[i].gameObject);
            else hair = hairs[i].gameObject;
        }
    }

    void PickToro()
    {
        var torsos = transform.FindAllRecursive("Torso", true);

        var torsoIndex = Random.Range(0, torsos.Count);

        for (int i = torsos.Count - 1; i >= 0; i--)
        {
            if (i != torsoIndex) GameObject.Destroy(torsos[i].gameObject);
            else torso = torsos[i].gameObject;
        }
    }

    void PlaceArms()
    {
        var arms = transform.FindAllRecursive("ArmUpper", true);

        foreach (var a in arms)
        {
            if (a.name.Contains(".L")) uarmL = a.gameObject;
            if (a.name.Contains(".R")) uarmR = a.gameObject;
        }

        var shoulders = torso.transform.FindAllRecursive("Shoulder", true);

        foreach (var s in shoulders)
        {
            if (s.name.Contains(".L")) shoulderL = s.gameObject;
            if (s.name.Contains(".R")) shoulderR = s.gameObject;
        }

        uarmL.transform.position = shoulderL.transform.position;
        uarmR.transform.position = shoulderR.transform.position;

        uarmL.transform.SetParent(shoulderL.transform);
        uarmR.transform.SetParent(shoulderR.transform);

        uarmL.transform.forward = -shoulderL.transform.right + Vector3.down;
        uarmR.transform.forward = shoulderR.transform.right + Vector3.down;
    }

    void HeadToTorso()
    {
        head.transform.SetParent(torso.transform);
    }

    void GetRemainingRefs()
    {
        var children = transform.GetChildrenDeep();
        foreach (var c in children)
        {
            if (c.name.Contains("ArmFore.L")) farmL = c.gameObject;
            else if (c.name.Contains("ArmFore.R")) farmR = c.gameObject;
            else if (c.name.Contains("LegUpper.L")) ulegL = c.gameObject;
            else if (c.name.Contains("LegUpper.R")) ulegR = c.gameObject;
            else if (c.name.Contains("LegLower.L")) llegL = c.gameObject;
            else if (c.name.Contains("LegLower.R")) llegR = c.gameObject;
            else if (c.name.Contains("Foot.L")) footL = c.gameObject;
            else if (c.name.Contains("Foot.R")) footR = c.gameObject;
            else if (c.name.Contains("Pelvis")) pelvis = c.gameObject;
        }
    }

    void ApplyMats()
    {
        var hairMat = hairMats.RandomOne();
        var skinMat = skinMats.RandomOne();
        var shirtMat = clothingMats.RandomOne();
        var pantsMat = clothingMats.RandomOne();
        var shoeMat = clothingMats.RandomOne();

        hair.GetComponent<MeshRenderer>().material = hairMat;

        torso.GetComponent<MeshRenderer>().material = shirtMat;
        uarmL.GetComponent<MeshRenderer>().material = shirtMat;
        uarmR.GetComponent<MeshRenderer>().material = shirtMat;

        farmL.GetComponent<MeshRenderer>().material = skinMat;
        farmR.GetComponent<MeshRenderer>().material = skinMat;
        head.GetComponent<MeshRenderer>().material = skinMat;
        ulegL.GetComponent<MeshRenderer>().material = skinMat;
        ulegR.GetComponent<MeshRenderer>().material = skinMat;
        llegL.GetComponent<MeshRenderer>().material = skinMat;
        llegR.GetComponent<MeshRenderer>().material = skinMat;

        pelvis.GetComponent<MeshRenderer>().material = pantsMat;

        footL.GetComponent<MeshRenderer>().material = shoeMat;
        footR.GetComponent<MeshRenderer>().material = shoeMat;
    }
    #endregion

    #region AnimationFunctions
    float walkCycle;
    public float walkSpeed = 2;

    float armTimer, armInterval;
    Vector3 lArmDir, rArmDir;
    void UArmsUpdate()
    {
        armTimer += Time.deltaTime;
        if (armTimer >= armInterval)
        {
            armTimer = 0;
            armInterval = Random.Range(.1f, 1.5f);
            bool left = Random.Range(0, 2) == 0 ? true : false;

            Vector3 dir = Random.insideUnitSphere.normalized;
            dir.x = Mathf.Abs(dir.x) * (left ? -1 : 1);
            dir.z = Mathf.Abs(dir.z);
            dir = transform.rotation * dir;

            if (left)
            {
                lArmDir = dir;
                rArmDir = transform.rotation * (Vector3.down * 4 + Vector3.right).normalized;
            }
            else
            {
                rArmDir = dir;
                lArmDir = transform.rotation * (Vector3.down * 4 + Vector3.left).normalized;
            }
        }

        uarmL.transform.rotation = Quaternion.RotateTowards(uarmL.transform.rotation, Quaternion.LookRotation(lArmDir), Time.deltaTime * 120f);
        uarmR.transform.rotation = Quaternion.RotateTowards(uarmR.transform.rotation, Quaternion.LookRotation(rArmDir), Time.deltaTime * 120f);
    }

    bool leftElbow;
    float farmTimer, farmInterval, elbowRate;
    void FArmsUpdate()
    {
        farmTimer += Time.deltaTime;
        if (farmTimer >= farmInterval)
        {
            farmTimer = 0;
            farmInterval = Random.Range(.5f, 2f);
            leftElbow = Random.Range(0, 2) == 0 ? true : false;
            elbowRate = Random.Range(2, 6f);
        }

        if (leftElbow)
        {
            farmL.transform.localRotation = Quaternion.Slerp(farmL.transform.localRotation, Quaternion.Euler(-60, 0, 0), Time.deltaTime * elbowRate);
            farmR.transform.localRotation = Quaternion.Slerp(farmR.transform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * elbowRate);
        }
        else
        {
            farmR.transform.localRotation = Quaternion.Slerp(farmR.transform.localRotation, Quaternion.Euler(-60, 0, 0), Time.deltaTime * elbowRate);
            farmL.transform.localRotation = Quaternion.Slerp(farmL.transform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * elbowRate);
        }
    }

    Vector3 headDir = Vector3.up;
    float headTimer, headInterval;
    void HeadBopUdate()
    {
        headTimer += Time.deltaTime;
        if (headTimer >= headInterval)
        {
            headTimer = 0;
            headInterval = Random.Range(.25f, 1);
            int dir = Random.Range(0, 5);
            if (dir == 0) headDir = Vector3.forward;
            else if (dir == 1) headDir = (3 * Vector3.forward + Vector3.down).normalized;
            else if (dir == 2) headDir = (3 * Vector3.forward + Vector3.up).normalized;
            else if (dir == 3) headDir = (3 * Vector3.forward + Vector3.left).normalized;
            else if (dir == 4) headDir = (3 * Vector3.forward + Vector3.right).normalized;
        }

        head.transform.localRotation = Quaternion.Slerp(head.transform.localRotation, Quaternion.LookRotation(headDir), Time.deltaTime * 2f);
    }
    #endregion
}
