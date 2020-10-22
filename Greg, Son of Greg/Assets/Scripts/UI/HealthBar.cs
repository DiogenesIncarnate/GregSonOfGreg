using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        slider.maxValue = player.maxHP;
        slider.value = player.currentHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.maxValue != player.maxHP) slider.maxValue = player.maxHP;
        if (slider.value != player.currentHP) slider.value = player.currentHP;
    }
}
