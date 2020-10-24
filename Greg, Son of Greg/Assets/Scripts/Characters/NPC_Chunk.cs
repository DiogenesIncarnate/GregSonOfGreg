using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Chunk : MonoBehaviour
{
    public List<NPC> NPCs;
    public Transform origin;
    

    [HideInInspector]
    public bool dialogueCompleted = false;
    [HideInInspector]
    public DialogueManager dialogueManager;

    // Start is called before the first frame update
    void Start()
    {
        origin = gameObject.transform;
        NPCs.Clear();
        NPCs.AddRange(gameObject.GetComponentsInChildren<NPC>());
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dialogueCompleted && NPCs.Count > 0 && !AnyHostile() && InDialogueRange() && dialogueManager.possibleConvos.Count >= 1)
        {
            DialogueManager.ToggleCharacters(false);
            DialogueManager.StartConversation(dialogueManager.possibleConvos[Random.Range(0, dialogueManager.possibleConvos.Count - 1)]);
            DialogueManager.SetInvolvedNPCs(NPCs);
            dialogueCompleted = true;
        }
    }

    public bool InDialogueRange()
    {
        Player player = FindObjectOfType<Player>();
        Vector2 dist = transform.position - player.transform.position;

        return (Mathf.Abs(dist.x) < 5.0f && Mathf.Abs(dist.y) < 1.0f);
    }

    public bool AnyHostile()
    {
        foreach(NPC npc in NPCs)
        {
            if (npc.GetInclination() == Inclination.Hostile) return true;
        }

        return false;
    }
}
