using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;
    public bool gameIsOver = false;
    public bool gameWin = false;
    public LivesManage livesManager;

    public customer[] customers;

    [Header("Music Settings")]
    public AudioSource musicSource;
    public AudioClip mainMusic;

    // Game win setup
    public GameObject playerToDestroy;
    public GameObject winToSpawn;
    private Transform spawnLocation;

    // Testing
    [ContextMenu("Test Game Win")]
    void TestGameWin()
    {
        GameWin();
    }

    [ContextMenu("Test Game Over")]
    void TestGameOver()
    {
        HandleGameOver();
    }

    void Start()
    {
        Application.targetFrameRate = 60;

        if (GM == null)
        {
            GM = this;
        }

        PlayBackgroundMusic();
    }

    void Update()
    {
        if (!gameIsOver && AllCustomersDone())
        {
            GameWin();
        }
    }

    void PlayBackgroundMusic()
    {
        if (musicSource != null && mainMusic != null)
        {
            musicSource.clip = mainMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    bool AllCustomersDone()
    {
        foreach (customer c in customers)
        {
            if (c == null || !c.isDone)
                return false;
        }
        return true;
    }

    public void GameWin()
    {
        Debug.Log("Game Win");
        gameIsOver = true;

        spawnLocation = PlayerController.Instance.GetCurrentRowTransform();

        if (playerToDestroy != null)
            Destroy(playerToDestroy);

        if (winToSpawn != null)
        {
            Instantiate(winToSpawn, spawnLocation.position, spawnLocation.rotation);
        }
    }

    public void HandleGameOver()
    {
        if (livesManager == null)
        {
            Debug.LogWarning("LivesManager is not assigned.");
            return;
        }

        if (livesManager.HasLivesLeft())
        {
            Debug.Log("Player lost a life. Pausing for animation...");
            Time.timeScale = 0f; // Pause game
            StartCoroutine(HandleLifeLoss());
        }
        else
        {
            Debug.Log("No lives left. Game Over.");
            gameIsOver = true;
            // Add game over UI, scene reload, etc. here
        }
    }

    private IEnumerator HandleLifeLoss()
    {
        yield return new WaitForSecondsRealtime(1.5f); // Replace with animation length if needed

        livesManager.LoseLife(); // Reduce life by 1

        Time.timeScale = 1f; // Resume game
        Debug.Log("Game resumed after losing a life.");
    }
}