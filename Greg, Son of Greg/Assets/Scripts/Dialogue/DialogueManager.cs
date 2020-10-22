using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public List<Conversation> possibleConvos;
    public TextMeshProUGUI speakerName, dialogue, navButtonText;
    public GameObject options;
    public GameObject buttonPrefab;
    public Image speakerSprite;

    private int currentIndex;
    private Conversation currentConvo;
    private List<NPC> currentInvolvedNPCs;
    private static DialogueManager instance;
    private Animator anim;
    private Coroutine typing;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            anim = GetComponent<Animator>();
            possibleConvos = CopyList(possibleConvos);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateThreatLevel(Conversation convo, ThreatLevel tone)
    {
        if(tone == ThreatLevel.Aggressive)
        {
            convo.threatLevel += 10;
        }
        else if(tone == ThreatLevel.Charming)
        {
            convo.threatLevel -= 5;
        }
    }

    public static void SetInvolvedNPCs(List<NPC> NPCs)
    {
        instance.currentInvolvedNPCs = NPCs;
    }

    public static void StartConversation(Conversation convo)
    {
        instance.anim.SetBool("IsOpen", true);
        instance.currentIndex = 0;
        instance.currentConvo = convo;
        instance.speakerName.text = "";
        instance.dialogue.text = "";
        instance.navButtonText.text = ">";

        instance.ReadNext();
    }

    public void ReadNext()
    {
        if(currentIndex > currentConvo.GetLength())
        {
            instance.anim.SetBool("IsOpen", false);
            currentConvo.complete = true;
            OnEnd(currentConvo);
            return;
        }

        speakerName.text = currentConvo.GetLineByIndex(currentIndex).speaker.GetName();
        DialogueLine currentLine = currentConvo.GetLineByIndex(currentIndex);
        string appropriateResponse = FindAppropriateResponse(currentLine);
        

        if (typing == null)
        {
            typing = instance.StartCoroutine(TypeText(appropriateResponse));
        }
        else
        {
            instance.StopCoroutine(typing);
            typing = null;
            typing = instance.StartCoroutine(TypeText(appropriateResponse));
        }

        speakerSprite.sprite = currentConvo.GetLineByIndex(currentIndex).speaker.GetSprite();
        currentIndex++;

        if(currentIndex > currentConvo.GetLength())
        {
            navButtonText.text = "X";
        }
    }

    private string FindAppropriateResponse(DialogueLine line)
    {
        if (currentConvo.threatLevel <= 5)
        {
            if (line.dialogueOptions.Length >= 1)
            return line.dialogueOptions[0];
        }
        else if (currentConvo.threatLevel >= 10)
        {
            if(line.dialogueOptions.Length >= 2)
            return line.dialogueOptions[1];
        }

        if (line.dialogueOptions.Length >= 3)
        {
            return line.dialogueOptions[2];
        }

        return line.dialogueOptions[0];
    }

    private void OnEnd(Conversation convo)
    {
        convo.complete = true;
        possibleConvos.Remove(convo);

        ToggleCharacters(true);

        Debug.Log(currentConvo.threatLevel);

        foreach(NPC npc in instance.currentInvolvedNPCs)
        {
            if(currentConvo.threatLevel >= 20)
            {
                npc.inclination = Inclination.Hostile;
            }
            else if(currentConvo.threatLevel >= 10)
            {
                npc.inclination = Inclination.Neutral;
            }
            else
            {
                npc.inclination = Inclination.Friendly;
            }
        }

        instance.currentInvolvedNPCs.Clear();
    }

    public static void ToggleCharacters(bool on)
    {
        foreach (DynamicCharacter ch in FindObjectsOfType<DynamicCharacter>())
        {
            ch.enabled = on;
            if (ch.gameObject.GetComponent<AIPath>())
            {
                ch.gameObject.GetComponent<AIPath>().enabled = on;
            }
        }
    }

    private IEnumerator TypeText(string text)
    {
        dialogue.text = "";
        bool complete = false;
        int index = 0;

        // Clear previous choices, if any
        foreach (Transform opt in options.transform)
        {
            Destroy(opt.gameObject);
        }

        if (currentConvo.GetLineByIndex(currentIndex).dialogueType == LineType.Choose)
        {
            string[] dOptions = currentConvo.GetLineByIndex(currentIndex).dialogueOptions;
            float yPos = 50;
            for (int i = 0; i < dOptions.Length; i++)
            {
                GameObject b = Instantiate(buttonPrefab);
                b.GetComponentInChildren<TextMeshProUGUI>().text = dOptions[i];
                b.transform.SetParent(options.transform);
                b.transform.localPosition = new Vector2(250, yPos);
                b.GetComponent<Button>().onClick.AddListener(ReadNext);
                if(i == 0)
                {
                    b.GetComponent<Button>().onClick.AddListener(delegate { UpdateThreatLevel(currentConvo, ThreatLevel.Charming); });
                }
                else if(i == 1)
                {
                    b.GetComponent<Button>().onClick.AddListener(delegate { UpdateThreatLevel(currentConvo, ThreatLevel.Aggressive); });
                }
                else
                {
                    b.GetComponent<Button>().onClick.AddListener(delegate { UpdateThreatLevel(currentConvo, ThreatLevel.Neutral); });
                }

                DialogueLine currentLine = currentConvo.GetLineByIndex(currentIndex);
                yPos -= 50;
            }
        }
        else if(currentConvo.GetLineByIndex(currentIndex).dialogueType == LineType.Respond)
        {
            while (!complete)
            {
                dialogue.text += text[index];
                index++;
                yield return new WaitForSeconds(0.02f);

                if (index == text.Length)
                {
                    complete = true;
                }
            }
        }

        typing = null;
    }

    private List<Conversation> CopyList(List<Conversation> list)
    {
        List<Conversation> copy = new List<Conversation>();
        foreach (Conversation c in list)
        {
            Conversation newC = c.Copy();
            copy.Add(newC);
        }

        return copy;
    }
}
