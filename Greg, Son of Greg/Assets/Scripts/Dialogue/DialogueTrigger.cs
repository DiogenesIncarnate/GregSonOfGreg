using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public List<Conversation> convos;
    public bool triggered = false;
    public bool automatic = true;
    public NPC attachedNPC;

    public void Start()
    {
        if(transform.parent.gameObject.GetComponent<NPC>())
        attachedNPC = transform.parent.gameObject.GetComponent<NPC>();
    }

    public void Update()
    {
        if (automatic)
        {
            if (Mathf.Abs(transform.position.x - GameObject.FindObjectOfType<Player>().transform.position.x) < 5.0f &&
                Mathf.Abs(transform.position.y - GameObject.FindObjectOfType<Player>().transform.position.y) < 1.0f)
            {
                StartConvo();
            }
        }
    }

    public void StartConvo()
    {
        DialogueManager.StartConversation(GetRandomConvo(convos));
    }

    public Conversation GetRandomConvo(List<Conversation> c)
    {
        int i = Random.Range(0, convos.Count - 1);
        return c[i];
    }
}
