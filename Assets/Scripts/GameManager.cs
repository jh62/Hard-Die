using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get => instance; }

    private static GameManager instance;

    [SerializeField] private PlayerController player;

    [SerializeField] private TextMeshProUGUI textEnemiesLeft;
    [SerializeField] private TextMeshProUGUI textPlayerHealth;
    [SerializeField] private TextMeshProUGUI textPlayerBullets;

    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioClip[] tracks;

    [SerializeField] GameObject enemies;

    [SerializeField] GameObject TextWin;
    [SerializeField] GameObject TextGameOver;

    private int enemyCount;
    private bool gameOver = false;

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

        StartCoroutine("MusicLoop");
    }

    private void Start()
    {
        foreach (var e in enemies.transform.GetComponentsInChildren<EnemyController>())
        {
            enemyCount++;
            e.OnDead += OnEnemyDead;
        }
    }


    void Update()
    {
        if (gameOver)
            return;

        float pHealth = Mathf.RoundToInt(((float)player.Health / player.MaxHealth) * 100f);
        textPlayerHealth.text = $"Health: {pHealth}%";
        textPlayerBullets.text = $"Bullets: {player.Inventory.GetWeapon().Bullets}";

        textEnemiesLeft.text = $"Enemies left: {enemyCount}";

        if (!player.isAlive())
        {
            gameOver = true;
            StopAllCoroutines();
            TextGameOver.SetActive(true);
            StartCoroutine("GoMainMenu", 5f);
            return;
        }

        if (enemyCount == 0)
        {
            gameOver = true;
            StopAllCoroutines();
            TextWin.SetActive(true);
            StartCoroutine("GoMainMenu", 5f);
            return;
        }
    }

    private void OnEnemyDead(BaseCharacter e)
    {
        e.OnDead -= OnEnemyDead;

        enemyCount -= 1;

        if (enemyCount < 0)
            enemyCount = 0;
    }

    IEnumerator GoMainMenu(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        yield break;
    }

    IEnumerator MusicLoop()
    {
        int musicIndex = Random.Range(0, tracks.Length - 1);

        while (!gameOver)
        {
            AudioClip clip = tracks[musicIndex++];
            musicPlayer.clip = clip;
            musicPlayer.Play();

            yield return new WaitForSeconds(clip.length);

            musicPlayer.Stop();

            if (musicIndex >= tracks.Length)
                musicIndex = 0;
        }
    }
}
