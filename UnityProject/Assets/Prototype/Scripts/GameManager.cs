using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : SecureSingleton<GameManager>
{
    [SerializeField] List<GameObject> enableOnStart;
    [SerializeField] List<GameObject> disableOnStart;

    [SerializeField] UnityEvent onPause;
    [SerializeField] UnityEvent onUnpause;

    public Image splashImage;
    public Image gameOverImage;

    bool paused = false;
    int mode = 0; // 0:splash, 1:play, 2:end
    PlayerController playerController;
    Transform playerTransform;

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
                    // Player input
                    if (Input.GetButtonDown("Jump")) { playerController.Jump(); }
                    playerController.HorizontalInput(Input.GetAxisRaw("Horizontal"));
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
    }

    void Unpause()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        onUnpause.Invoke();
    }

    void Clock1()
    {
        if (playerTransform != null)
        {
            if (playerTransform.position.y < -50)
            {
                playerController.ResetPosition();
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    public static void RegisterPlayer(PlayerController playerController)
    {
        This.playerController = playerController;
        This.playerTransform = playerController.transform;
    }

    public static void GameOver()
    {
        This.mode = 2;
        This.Pause();
        This.gameOverImage.gameObject.SetActive(true);
    }

    void OnValidate()
    {
        enableOnStart.RemoveAll(x => x == null);
        disableOnStart.RemoveAll(x => x == null);
    }

    void OnDrawGizmos()
    {
        // Draw rest box at -50 on y axis
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(new Vector3(0, -300, 0), new Vector3(5000, 500, 0));

    }
}