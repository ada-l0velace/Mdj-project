using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitTowerGUI : IGUI {
    Turret tower;
    GameObject current;
    public UnitTowerGUI(Turret tower, GameObject current) {
        this.tower = tower;
        this.current = current;
        current.GetComponentsInChildren<Button>()[0].onClick.AddListener(tower.SellTurret);
        //current.GetComponentsInChildren<Button>()[1].onClick.AddListener(tower.SellTurret);
    }

    public void UpdateStats() {
        if (!tower)
            return;
        Bullet bullet = tower.bulletPrefab.GetComponent<Bullet>();
        var texts = current.GetComponentsInChildren<Text>();
        
        texts[3].text = tower.name;
        texts[0].text = "Type: " +  tower.eType.ToString();
        texts[1].text = "Damage: E" + bullet.GetDamageEarth() + " F" + bullet.GetDamageFire() + " W" + bullet.GetDamageWater() + " I" + bullet.GetDamageIce();
        current.transform.SetAsLastSibling();
        //texts[2].text = "Resistance: ";
    }

    public void ActivateUI() {
        current.SetActive(true);
        UpdateStats();
    }

    public void DeactivateUI() {
        current.SetActive(false);
    }
}
