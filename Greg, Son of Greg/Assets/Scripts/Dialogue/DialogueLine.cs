using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LineType {
    Choose,
    Respond,
}

public enum ThreatLevel
{
    Aggressive,
    Neutral,
    Charming,
}

[System.Serializable]
public class DialogueLine
{
    public Speaker speaker;
    public LineType dialogueType = LineType.Respond;
    [TextArea]
    public string[] dialogueOptions;
    public ThreatLevel[] threatLevels;
}
