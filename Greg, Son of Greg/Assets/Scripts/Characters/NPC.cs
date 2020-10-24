using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public enum Inclination
{
    Friendly,
    Neutral,
    Hostile,
}

public class NPC : DynamicCharacter
{
    public bool defaultHostile = false;
    public List<DynamicCharacter> hostiles;
    public DynamicCharacter closestHostile;
    public float awarenessRadius = 10.0f;
    private Inclination inclination = Inclination.Neutral;
    public GameObject parentChunk;
    public Transform origin;

    public void SetInclination(Inclination i)
    {
        inclination = i;
        UpdateHostiles();
    }

    public Inclination GetInclination()
    {
        return inclination;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        origin = gameObject.transform;

        if(parentChunk == null)
        {
            parentChunk = gameObject.GetComponentInParent<NPC_Chunk>().gameObject;
        }

        InitHostiles();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if(inclination != Inclination.Neutral && !isDead && hostiles.Count > 0)
        {
            ActHostile();
        }
        else
        {
            StopFollowHostile();
        }
    }

    public void InitHostiles()
    {
        hostiles = new List<DynamicCharacter>();

        if (defaultHostile)
        {
            inclination = Inclination.Hostile;
            hostiles.Add(FindObjectOfType<Player>());
        }

        closestHostile = GetClosestHostile();

        if (closestHostile)
        {
            gameObject.GetComponent<AIDestinationSetter>().target = closestHostile.transform;
        }
    }

    public void ActHostile()
    {
        closestHostile = GetClosestHostile();
        Vector2 rel = RelativeToPosition(closestHostile.transform.position);

        if (InCombatRange())
        {
            FaceHostile(rel.x);
            StartFollowHostile();

            if (CheckAttackRange())
            {
                OnAttackInput();
            }
        }
        else
        {
            StopFollowHostile();
        }
    }

    public DynamicCharacter GetClosestHostile()
    {
        if (hostiles.Count <= 0) return null;

        DynamicCharacter closest = hostiles[0];
        foreach(DynamicCharacter h in hostiles)
        {
            float closestDist = Vector2.Distance(closest.transform.position, transform.position);
            float thisDist = Vector2.Distance(h.transform.position, transform.position);
            if (thisDist < closestDist) closest = h;
        }

        return closest;
    }

    public void UpdateHostiles()
    {
        hostiles.Clear();

        if(inclination == Inclination.Friendly)
        {
            NPC[] allNPC = FindObjectsOfType<NPC>();
            foreach (NPC n in allNPC)
            {
                if (n.GetInclination() == Inclination.Hostile)
                {
                    hostiles.Add(n);
                }
            }
        }
        else if(inclination == Inclination.Hostile)
        {
            hostiles.Add(FindObjectOfType<Player>());
        }

        closestHostile = GetClosestHostile();

        if (closestHostile)
        {
            gameObject.GetComponent<AIDestinationSetter>().target = closestHostile.transform;
        }
    }

    public void RemoveFromHostiles(DynamicCharacter g)
    {
        int indexToRemove = -1;

        foreach(DynamicCharacter h in hostiles)
        {
            if(h.gameObject.GetInstanceID() == g.gameObject.GetInstanceID())
            {
                indexToRemove = hostiles.IndexOf(h);
                break;
            }
        }

        if(indexToRemove >= 0)
        {
            hostiles.RemoveAt(indexToRemove);
        }
    }

    public bool IsAHostile(DynamicCharacter g)
    {
        foreach (DynamicCharacter h in hostiles)
        {
            if (h.gameObject.GetInstanceID() == g.gameObject.GetInstanceID())
            {
                return true;
            }
        }

        return false;
    }

    public bool InCombatRange()
    {
        Vector2 d = RelativeToPosition(closestHostile.transform.position);
        return !isDead && Mathf.Abs(d.y) < 3.0f && Mathf.Abs(d.x) < 10.0f;
    }

    public bool CheckAttackRange()
    {
        Vector2 d = RelativeToPosition(closestHostile.transform.position);
        return Mathf.Abs(d.y) < 2.0f && Mathf.Abs(d.x) < 1.0f;
    }

    public void FaceHostile(float faceDir)
    {
        Vector3 tempScale = transform.localScale;
        tempScale.x = Mathf.Abs(tempScale.x) * Mathf.Sign(faceDir);
        transform.localScale = tempScale;
    }

    public void StartFollowHostile()
    {
        gameObject.GetComponent<AIDestinationSetter>().target = closestHostile.transform;
        gameObject.GetComponent<AIPath>().canSearch = true;
        gameObject.GetComponent<AIPath>().canMove = true;
    }

    public void StopFollowHostile()
    {
        if (parentChunk.GetComponent<NPC_Chunk>() && !isDead)
        {
            //parentChunk.GetComponent<NPC_Chunk>().origin;
            gameObject.GetComponent<AIDestinationSetter>().target = origin;
        }
        else
        {
            gameObject.GetComponent<AIPath>().canSearch = false;
            gameObject.GetComponent<AIPath>().canMove = false;
        }
    }

    Vector2 RelativeToPosition(Vector3 p)
    {
        return p - transform.position;
    }
}
