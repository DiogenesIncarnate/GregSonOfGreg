using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum Stat
{
    Might, 
    Charm,
    Wit,
}

[RequireComponent(typeof(TextMeshProUGUI))]
public class StatsTracker : MonoBehaviour
{
    private TextMeshProUGUI textMesh;

    public Stat stat;
    [HideInInspector]
    public Player player;

    void Start()
    {
        textMesh = gameObject.GetComponent<TextMeshProUGUI>();
        player = FindObjectOfType<Player>();
        InvokeRepeating("UpdateStat", 0, 2.0f);
    }

    void UpdateStat()
    {
        if (stat == Stat.Might) textMesh.SetText(player.might.ToString());
        else if (stat == Stat.Charm) textMesh.SetText(player.charm.ToString());
        else if (stat == Stat.Wit) textMesh.SetText(player.wit.ToString());
    }
}
