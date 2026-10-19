using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using TMPro;
using StarterAssets; // เพิ่มส่วนนี้เพื่อดึง StarterAssetsInputs มาใช้งาน
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Hearts")]
    [SerializeField] private int maxHearts = 3;
    private int currentHearts;

    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerSpawnPoint;

    [Header("Hunter")]
    [SerializeField] private Transform hunter;
    [SerializeField] private Transform hunterSpawnPoint;

    [Header("UI")]
    [SerializeField] private TMP_Text heartText;
    [SerializeField] private GameObject gameOverPanel;

    private CharacterController playerController;
    private StarterAssetsInputs playerInputs; // สคริปต์ควบคุมเมาส์ของ Starter Assets
    private NavMeshAgent hunterAgent;
    private float nextCatchTime;
    private bool gameOver;


    public bool IsGameOver => gameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Time.timeScale = 1f;
        currentHearts = maxHearts;
    }

    private void Start()
    {
        if (player != null)
        {
            playerController = player.GetComponent<CharacterController>();
            playerInputs = player.GetComponent<StarterAssetsInputs>(); // ดึงคอมโพเนนต์ StarterAssetsInputs
        }

        if (hunter != null)
            hunterAgent = hunter.GetComponent<NavMeshAgent>();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateHeartUI();
    }

    private void Update()
    {
        // ตรวจจับการกด Enter เมื่ออยู่ในหน้า Game Over
        if (gameOver && Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            ResetNewGame();
        }
    }

    public void PlayerCaught()
    {
        if (gameOver || Time.time < nextCatchTime)
            return;

        nextCatchTime = Time.time + 2f;
        currentHearts--;

        UpdateHeartUI();

        if (currentHearts <= 0)
        {
            EndGame();
            return;
        }

        RespawnPlayer();
        ResetHunter();
        
         // เติมบรรทัดนี้: ล้างกระเป๋า + ไอเทมกลับมา + ผีหายไป
        FindFirstObjectByType<HitReset>()?.ResetAfterHit();
    }

    private void RespawnPlayer()
    {
        if (player == null || playerSpawnPoint == null)
        {
            Debug.LogError("กรุณากำหนด Player และ Player Spawn Point");
            return;
        }

        if (playerController != null)
            playerController.enabled = false;

        player.SetPositionAndRotation(
            playerSpawnPoint.position,
            playerSpawnPoint.rotation
        );

        if (playerController != null)
            playerController.enabled = true;
    }

    private void ResetHunter()
    {
        if (hunter == null || hunterSpawnPoint == null)
            return;

        if (hunterAgent != null)
        {
            hunterAgent.Warp(hunterSpawnPoint.position);
            hunterAgent.ResetPath();
            hunter.rotation = hunterSpawnPoint.rotation;
        }
        else
        {
            hunter.SetPositionAndRotation(
                hunterSpawnPoint.position,
                hunterSpawnPoint.rotation
            );
        }
    }

    private void UpdateHeartUI()
    {
        if (heartText != null)
        {
            heartText.text = new string('♥', currentHearts);
        }
    }


    private void EndGame()
    {
        gameOver = true;
        StarterAssets.StarterAssetsInputs.gameOverCursor = true;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }


    public void ResetNewGame()
    {
        Time.timeScale = 1f;

        StarterAssetsInputs.gameOverCursor = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (sceneIndex < 0)
        {
            Debug.LogError(
                "Scene is not in Build Profiles / Build Settings."
            );
            return;
        }

        SceneManager.LoadScene(sceneIndex);
    }
}