using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitEnemyGUI : IGUI {
    Enemy enemy;
    GameObject current;

    public UnitEnemyGUI(Enemy enemy, GameObject current) {
        this.enemy = enemy;
        this.current = current;
    }

    public string GetName() {
        return enemy.name;
    }

    public float GetSpeed() {
        if (enemy.m_Agent)
            return enemy.m_Agent.speed;
        return 0f;
    }

    public void UpdateStats() {
        var texts = current.GetComponentsInChildren<Text>();
        texts[3].text = "Cube";
        texts[0].text = "Type: " + enemy.eType.ToString();
        texts[1].text = "Speed: " + GetSpeed().ToString();
        texts[2].text = "Resistance: ";
    }

    public void UpdateHealthBar() {
        var slider = current.GetComponentInChildren<Slider>();
        slider.value = enemy.healthBar.value;
    }

    public void ActivateUI() {
        current.SetActive(true);
        UpdateHealthBar();
        UpdateStats();
    }

    public void DeactivateUI() {
        current.SetActive(false);
    }
}
