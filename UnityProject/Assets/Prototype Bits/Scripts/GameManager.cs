using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : SecureSingleton<GameManager>
{
    [SerializeField] List<GameObject> enableOnStart;
    [SerializeField] List<GameObject> disableOnStart;

    [SerializeField] UnityEvent onPause;
    [SerializeField] UnityEvent onUnpause;

    [Tooltip("If the player falls below this y-value, their position will be reset to the last checkpoint (or the beginning of the level if the player has not reached a checkpoint)")]
    public float deathHeight = -50f;

    [Tooltip("If checked the player has a limited amount of lives, decremented each time the player dies & resets to the last checkpoint. If the player runs out of lives a Game Over screen appears")]
    public bool limitPlayerLives = false;
    public int playerLives = 5;

    public Image splashImage;
    public Image gameOverImage;
    public Image gameCompleteImage;
    TextMeshProUGUI playerLivesText;

    bool paused = false;
    int mode = 0; // 0:splash, 1:play, 2:end
    PlayerController playerController;
    Transform playerTransform;

    private int checkpointIndex = -1; //index to determine which checkpoint is active

    protected override void Awake()
    {
        base.Awake();

        foreach (var o in enableOnStart)
        {
            if (o != null)
            {
                o.SetActive(true);
            }
        }

        foreach (var o in disableOnStart)
        {
            if (o != null)
            {
                o.SetActive(false);
            }
        }

        playerLivesText = transform.Find("Canvas").transform.Find("LivesText").GetComponent<TextMeshProUGUI>();

        Time.fixedDeltaTime = 1 / 100f;
        gameOverImage.color = new Color(1, 1, 1, 0);
        mode = 0;
        Unpause();

        InvokeRepeating(nameof(Clock1), 1, 1);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    void Update()
    {
        switch (mode)
        {
            case 0:
                if (Input.anyKeyDown || Input.GetButtonDown("Jump"))
                {
                    splashImage.gameObject.GetComponent<FadeOutPanel>().enabled = true;
                    mode = 1;
                }
                break;

            case 1:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    paused = !paused;

                    if (paused)
                    {
                        Pause();
                    }
                    else
                    {
                        Unpause();
                    }
                }

                if (!paused)
                {
                    //Player Lives
                    if(playerLives <= 0)
                    {
                        GameOver();
                    }
                    playerLivesText.text = "Lives: " + playerLives;


                    // Player input
                    if (Input.GetButtonDown("Jump")) { playerController.Jump(); }
                    playerController.HorizontalInput(Input.GetAxisRaw("Horizontal"));
                    if (Input.GetKeyDown(KeyCode.R)) { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
                }
                break;

            case 2:
                if (Input.anyKeyDown)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                break;
        }
    }

    void Pause()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        onPause.Invoke();
        playerLivesText.gameObject.SetActive(false);
    }

    void Unpause()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        onUnpause.Invoke();
        playerLivesText.gameObject.SetActive(true);
    }

    void Clock1()
    {
        if (playerTransform != null)
        {
            if (playerTransform.position.y < -50)
            {
                ResetLevel();
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    public static void RegisterPlayer(PlayerController playerController)
    {
        This.playerController = playerController;
        This.playerTransform = playerController.transform;
    }

    public static void ResetLevel()
    {
        This.playerController.ResetPosition();
        This.playerLives--;
    }

    public static void SetCheckpoint(int index)
    {
        This.playerController.SetCheckpoint();
        This.checkpointIndex = index;
    }

    public static void AddLives(int lives)
    {
        This.playerLives += lives;
    }

    public static void GameOver()
    {
        This.mode = 2;
        Time.timeScale = 0;
        Cursor.visible = true;
        This.playerLivesText.gameObject.SetActive(false);
        This.gameOverImage.gameObject.SetActive(true);
    }

    public static void GameComplete()
    {
        This.mode = 2;
        Time.timeScale = 0;
        Cursor.visible = true;
        This.playerLivesText.gameObject.SetActive(false);
        This.gameCompleteImage.gameObject.SetActive(true);
    }

    void OnValidate()
    {
        enableOnStart.RemoveAll(x => x == null);
        disableOnStart.RemoveAll(x => x == null);
    }

    void OnDrawGizmos()
    {
        // Draw line at deathHeight
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawLine(new Vector3(-5000, deathHeight, 0), new Vector3(5000, deathHeight, 0));

    }
}