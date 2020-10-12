using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Chunk : MonoBehaviour
{
    public List<NPC> NPCs;
    public Transform origin;
    public List<Conversation> possibleConvos;

    [HideInInspector]
    public bool dialogueCompleted = false;

    // Start is called before the first frame update
    void Start()
    {
        origin = gameObject.transform;
        NPCs.Clear();
        NPCs.AddRange(gameObject.GetComponentsInChildren<NPC>());
        possibleConvos = CopyList(possibleConvos);
    }

    // Update is called once per frame
    void Update()
    {
        if (!dialogueCompleted && InDialogueRange())
        {
            dialogueCompleted = true;
            DialogueManager.ToggleCharacters(false);
            DialogueManager.StartConversation(possibleConvos[Random.Range(0, possibleConvos.Count - 1)]);
            DialogueManager.SetInvolvedNPCs(NPCs);
        }
    }

    public bool InDialogueRange()
    {
        Player player = FindObjectOfType<Player>();
        Vector2 dist = transform.position - player.transform.position;

        return (Mathf.Abs(dist.x) < 5.0f && Mathf.Abs(dist.y) < 1.0f);
    }

    private List<Conversation> CopyList(List<Conversation> list)
    {
        List<Conversation> copy = new List<Conversation>();
        foreach(Conversation c in list)
        {
            Conversation newC = c.Copy();
            copy.Add(newC);
        }

        return copy;
    }
}
