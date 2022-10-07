using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get => instance; }

    private static GameManager instance;

    [SerializeField] private PlayerController player;

    [SerializeField] private TextMeshProUGUI textEnemiesLeft;
    [SerializeField] private TextMeshProUGUI textPlayerHealth;
    [SerializeField] private TextMeshProUGUI textPlayerBullets;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Update()
    {
        float pHealth = Mathf.RoundToInt(((float)player.Health / player.MaxHealth) * 100f);
        textPlayerHealth.text = $"Health: {pHealth}%";
        textPlayerBullets.text = $"Bullets: {player.Inventory.GetWeapon().Bullets}";
        // textPlayerHealth.text = $"{player.Health} / {player.MaxHealth * 100}";
    }
}
