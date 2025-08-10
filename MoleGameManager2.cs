using UnityEngine;
using TMPro;  // TextMeshPro用名前空間

public class MoleGameManager : MonoBehaviour
{
    public GameObject[] holes;  // 穴のオブジェクト配列
    public GameObject molePrefab;  // モグラのプレハブ
    public TextMeshProUGUI timerText;  // タイマー用テキスト (TextMeshPro)
    public TextMeshProUGUI scoreText;  // スコア用テキスト (TextMeshPro)

    private float gameTime = 10f;  // ゲーム時間（秒）
    private int score = 0;  // スコア
    private bool isGameActive = true;  // ゲームがアクティブかどうか

    void Start()
    {
        // 穴のオブジェクトを自動検出
        AutoDetectHoles();
        
        // UI要素の存在確認
        ValidateUIElements();
        
        // ゲーム開始
        StartGame();
    }

    void AutoDetectHoles()
    {
        // holes配列が空またはnullの場合、自動検出を試行
        if (holes == null || holes.Length == 0)
        {
            Debug.Log("MoleGameManager: Holes array is empty, attempting auto-detection...");
            
            // まず名前で検索を試行
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            System.Collections.Generic.List<GameObject> holeList = new System.Collections.Generic.List<GameObject>();
            
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.ToLower().Contains("hole") || 
                    obj.name.ToLower().Contains("穴") ||
                    obj.name.ToLower().Contains("hole") ||
                    obj.name.ToLower().Contains("hole"))
                {
                    holeList.Add(obj);
                    Debug.Log("MoleGameManager: Found potential hole: " + obj.name);
                }
            }
            
            if (holeList.Count > 0)
            {
                holes = holeList.ToArray();
                Debug.Log("MoleGameManager: Found " + holes.Length + " holes by name search.");
            }
            else
            {
                // 名前で見つからない場合は、子オブジェクトとして穴を作成
                Debug.Log("MoleGameManager: No holes found by name. Creating default holes...");
                CreateDefaultHoles();
            }
        }
        else
        {
            Debug.Log("MoleGameManager: Holes array is already configured with " + holes.Length + " holes.");
        }
    }

    void CreateDefaultHoles()
    {
        // デフォルトの穴を3x3のグリッドで作成
        holes = new GameObject[9];
        float spacing = 2f;
        int index = 0;
        
        for (int x = -2; x <= 2; x += 2)
        {
            for (int z = -2; z <= 2; z += 2)
            {
                GameObject hole = new GameObject("Hole_" + index);
                hole.transform.SetParent(transform);
                hole.transform.localPosition = new Vector3(x, 0, z);
                
                // 穴の見た目を作成（オプション）
                GameObject holeVisual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                holeVisual.transform.SetParent(hole.transform);
                holeVisual.transform.localPosition = Vector3.zero;
                holeVisual.transform.localScale = new Vector3(1f, 0.1f, 1f);
                
                // マテリアルを設定
                Renderer renderer = holeVisual.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.black;
                }
                
                holes[index] = hole;
                index++;
            }
        }
        
        Debug.Log("MoleGameManager: Created " + holes.Length + " default holes in 3x3 grid.");
    }

    void ValidateUIElements()
    {
        if (timerText == null)
        {
            Debug.LogError("MoleGameManager: timerText is not assigned! Please assign it in the inspector.");
        }
        
        if (scoreText == null)
        {
            Debug.LogError("MoleGameManager: scoreText is not assigned! Please assign it in the inspector.");
        }
        
        if (holes == null || holes.Length == 0)
        {
            Debug.LogError("MoleGameManager: holes array is still empty after auto-detection!");
        }
        
        if (molePrefab == null)
        {
            Debug.LogError("MoleGameManager: molePrefab is not assigned! Please assign it in the inspector.");
        }
    }

    void StartGame()
    {
        // 穴が設定されていない場合はゲームを開始しない
        if (holes == null || holes.Length == 0)
        {
            Debug.LogError("MoleGameManager: Cannot start game - no holes available!");
            return;
        }
        
        // ゲーム状態をリセット
        gameTime = 10f;
        score = 0;
        isGameActive = true;
        
        // スコア表示を初期化
        if (scoreText != null)
        {
            scoreText.text = "Score: 0";
        }
        
        // モグラ生成を開始
        InvokeRepeating("SpawnMole", 0f, 1f);
        
        Debug.Log("MoleGameManager: Game started! Spawning moles every 1 second.");
    }

    void Update()
    {
        // ゲームがアクティブでない場合は何もしない
        if (!isGameActive) return;
        
        // UI要素が存在する場合のみ更新
        if (timerText != null)
        {
            // ゲーム時間をカウントダウン
            gameTime -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.Ceil(gameTime).ToString();

            if (gameTime <= 0)
            {
                GameOver();
            }
        }
    }

    void GameOver()
    {
        isGameActive = false;
        CancelInvoke("SpawnMole");
        
        if (timerText != null)
        {
            timerText.text = "Game Over!";
        }
        
        Debug.Log("MoleGameManager: Game Over! Final Score: " + score);
    }

    void SpawnMole()
    {
        // ゲームがアクティブでない場合は何もしない
        if (!isGameActive) return;
        
        // 必要な要素が設定されているか確認
        if (holes == null || holes.Length == 0)
        {
            Debug.LogError("MoleGameManager: Cannot spawn mole - holes array is empty.");
            return;
        }
        
        if (molePrefab == null)
        {
            Debug.LogError("MoleGameManager: Cannot spawn mole - molePrefab is not assigned.");
            return;
        }

        try
        {
            // ランダムな穴にモグラを出現させる
            int randomIndex = Random.Range(0, holes.Length);
            GameObject selectedHole = holes[randomIndex];
            
            if (selectedHole == null)
            {
                Debug.LogError("MoleGameManager: Selected hole is null at index " + randomIndex);
                return;
            }

            GameObject newMole = Instantiate(molePrefab, selectedHole.transform.position, Quaternion.identity);
            
            if (newMole == null)
            {
                Debug.LogError("MoleGameManager: Failed to instantiate mole prefab.");
                return;
            }

            // MoleスクリプトにMoleGameManagerを渡す
            Mole moleScript = newMole.GetComponent<Mole>();
            if (moleScript != null)
            {
                moleScript.SetGameManager(this);
                Debug.Log("MoleGameManager: Mole spawned successfully at hole " + randomIndex);
            }
            else
            {
                Debug.LogError("MoleGameManager: Mole script not found on instantiated mole prefab! Destroying invalid mole.");
                Destroy(newMole);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("MoleGameManager: Error spawning mole: " + e.Message);
        }
    }

    public void MoleHit()
    {
        // ゲームがアクティブでない場合は何もしない
        if (!isGameActive) return;
        
        // モグラが叩かれた時の処理
        score++;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
        else
        {
            Debug.LogWarning("MoleGameManager: scoreText is null, cannot update score display.");
        }
        
        Debug.Log("MoleGameManager: Mole hit! Score: " + score);
    }
}