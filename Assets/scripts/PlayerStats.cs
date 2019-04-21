using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStats : MonoBehaviour {

    public Text LivesGUI;
    public Text MoneyGUI;

    public static int Money;
    public int startMoney = 100;

    public static int Lives;
    public int startLives = 5;

    public static int Rounds;

    void Start() {
        Money = startMoney;
        Lives = startLives;

        Rounds = 0;
    }

    private void Update() {
        LivesGUI.text = "Lives: " + Lives.ToString();
        MoneyGUI.text = "Money: " + Money.ToString();
    }

}