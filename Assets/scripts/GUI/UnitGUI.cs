using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitGUI : MonoBehaviour {
    public static Enemy enemy;
    public GameObject current;
    
    public string GetName() {
        return enemy.name;
    }

    public float GetSpeed() {
        return enemy.m_Agent.speed;
    }

    public void UpdateStats() {
        var texts = current.GetComponentsInChildren<Text>();
        texts[3].text = "Cube";
        texts[1].text = "Speed: "+GetSpeed().ToString();
        texts[2].text = "Resistance: ";
    }

    public void UpdateHealthBar() {
        var slider = current.GetComponentInChildren<Slider>();
        slider.value = enemy.health / enemy.startHealth;
    }

    void Update() {
        if (enemy != null) {
            current.SetActive(true);
            UpdateHealthBar();
            UpdateStats();
        }
        else {
            current.SetActive(false);
        }
    }


}
