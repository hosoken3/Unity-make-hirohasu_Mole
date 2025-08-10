using UnityEngine;

public class Mole : MonoBehaviour
{
    private MoleGameManager gameManager;
    private bool isInitialized = false;

    void Start()
    {
        // 自動的にMoleGameManagerを探して設定
        InitializeGameManager();
    }

    void InitializeGameManager()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<MoleGameManager>();
            if (gameManager != null)
            {
                isInitialized = true;
                Debug.Log("Mole: GameManager found and initialized successfully.");
            }
            else
            {
                Debug.LogError("Mole: MoleGameManager not found in scene! Make sure it exists.");
            }
        }
    }

    // MoleGameManagerを設定するメソッド（手動設定用）
    public void SetGameManager(MoleGameManager manager)
    {
        gameManager = manager;
        isInitialized = true;
        Debug.Log("Mole: GameManager set manually.");
    }

    void OnMouseDown()
    {
        // 初期化されていない場合は再試行
        if (!isInitialized && gameManager == null)
        {
            InitializeGameManager();
        }

        // gameManagerがnullでないことを確認
        if (gameManager != null)
        {
            // モグラがクリックされた時の処理
            gameManager.MoleHit();
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("Mole: gameManager is still null! Check if MoleGameManager exists in scene.");
        }
    }
}