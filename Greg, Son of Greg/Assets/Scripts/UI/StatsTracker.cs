using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Stat
{
    Might, 
    Charm,
    Wit,
}

public class StatsTracker : MonoBehaviour
{
    public Stat stat;
    public TextMeshProUGUI value;
    public Slider progress;
    [HideInInspector]
    public Player player;

    void Start()
    {
        player = FindObjectOfType<Player>();
        InvokeRepeating("UpdateStat", 0, 2.0f);
    }

    void UpdateStat()
    {
        if (value)
        {
            if (stat == Stat.Might) value.SetText(((int)player.might).ToString());
            else if (stat == Stat.Charm) value.SetText(((int)player.charm).ToString());
            else if (stat == Stat.Wit) value.SetText(((int)player.wit).ToString());
        }
        if (progress)
        {
            if (stat == Stat.Might) progress.value = player.might % 1;
            else if (stat == Stat.Charm) progress.value = player.charm % 1;
            else if (stat == Stat.Wit) progress.value = player.wit % 1;
        }
    }
}
