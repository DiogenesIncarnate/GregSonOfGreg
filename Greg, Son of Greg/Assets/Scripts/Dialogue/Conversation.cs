#pragma warning disable 0649
using UnityEngine;

[CreateAssetMenu(fileName = "New Conversation", menuName = "Dialogue/New Conversation")]
public class Conversation : ScriptableObject
{
    public float threatLevel = 0;
    public bool complete = false;
    [SerializeField]
    private DialogueLine[] allLines;

    public DialogueLine GetLineByIndex(int index)
    {
        return allLines[index];
    }

    public int GetLength()
    {
        return allLines.Length - 1;
    }

    public Conversation Copy()
    {
        Conversation copy = new Conversation();
        copy.threatLevel = this.threatLevel;
        copy.complete = this.complete;
        copy.allLines = allLines;

        return copy;
    }
}
