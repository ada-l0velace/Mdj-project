using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitTowerGUI : IGUI {
    Turret tower;
    GameObject current;
    Button sellButton;
    Button upgradeButton;

    Text[] texts;

    public UnitTowerGUI(Turret tower, GameObject current) {
        this.tower = tower;
        this.current = current;
        sellButton = current.GetComponentsInChildren<Button>()[0];
        upgradeButton = current.GetComponentsInChildren<Button>()[1];
        texts = current.GetComponentsInChildren<Text>();

        //current.GetComponentsInChildren<Button>()[1].onClick.AddListener(tower.SellTurret);
    }

    public void UpdateStats() {
        if (!tower) {
            return;
        }
        tower.UpdateStats(texts);
        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(tower.SellTurret);
        current.transform.SetAsLastSibling();
    }

    public void ActivateUI() {
        current.SetActive(true);
        UpdateStats();
    }

    public void DeactivateUI() {
        current.SetActive(false);
    }
}
