using System.Collections.Generic;
using UnityEngine;

public class PuzzleLoader : MonoBehaviour
{
    public static PuzzleLoader Instance { get; private set; }

    [Header("Prefabs / Prototypes")]
    public GameObject heroPrefab;
    public GameObject enemyPrefab;
    public GameObject lanternPrefab;
    public GameObject obstaclePrefab;

    [Header("Current Level Index")]
    public int currentLevelIndex = 0;

    // List of level layouts:
    // . = Empty, H = Hero, E = Enemy, L = Lantern, X = Obstacle
    // Note: Array elements are defined from Y=0 (bottom) to top Y
    private readonly List<string[]> levels = new List<string[]>()
    {
        // --- LEVEL 1: Intro / Tutorial ---
        new string[]
        {
            ". . . H . .", // Y=0
            ". . . . . .",
            ". . . L . .",
            ". . . . . .",
            ". . . . . .",
            ". . . E . ."  // Y=5
        },

        // --- LEVEL 2: Chokepoint ---
        new string[]
        {
            ". . . H . .",
            ". X . . . .",
            ". . . L . .",
            "X X . X X X",
            ". . . . . .",
            "E . . . . E"
        },

        // --- LEVEL 3: Flank Attack ---
        new string[]
        {
            ". . H . . .",
            ". . . . . .",
            ". E . L . E",
            ". . . . . .",
            ". X . . . X",
            ". . . E . ."
        }
    };

    private void Awake()
    {
        Instance = this;
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Count)
        {
            Debug.Log("No more levels! You beat the game.");
            return;
        }

        ClearBoard();
        currentLevelIndex = levelIndex;
        string[] layout = levels[currentLevelIndex];
        float tileSize = BoardManager.Instance.tileSize;

        for (int y = 0; y < layout.Length; y++)
        {
            string[] tokens = layout[y].Split(' ');

            for (int x = 0; x < tokens.Length; x++)
            {
                string token = tokens[x];
                Vector2Int pos = new Vector2Int(x, y);

                switch (token)
                {
                    case "H":
                        SpawnHero(pos, tileSize);
                        break;
                    case "E":
                        SpawnEnemy(pos, tileSize);
                        break;
                    case "L":
                        SpawnLantern(pos, tileSize);
                        break;
                    case "X":
                        SpawnObstacle(pos, tileSize);
                        break;
                }
            }
        }

        Debug.Log($"Loaded Level {currentLevelIndex + 1}");
    }

    public void LoadNextLevel()
    {
        LoadLevel(currentLevelIndex + 1);
    }

    private void ClearBoard()
    {
        BoardManager board = BoardManager.Instance;
        if (board == null) return;

        // Destroy existing GameObjects
        foreach (var hero in board.heroes) if (hero != null) Destroy(hero.gameObject);
        foreach (var enemy in board.enemies) if (enemy != null) Destroy(enemy.gameObject);
        if (board.lantern != null) Destroy(board.lantern.gameObject);

        // Clear references
        board.heroes.Clear();
        board.enemies.Clear();
        board.obstacles.Clear();
        board.lantern = null;
    }

    private void SpawnHero(Vector2Int pos, float tileSize)
    {
        GameObject go = Instantiate(heroPrefab);
        HeroController hero = go.GetComponent<HeroController>();
        hero.InitPosition(pos, tileSize);
        BoardManager.Instance.heroes.Add(hero);
    }

    private void SpawnEnemy(Vector2Int pos, float tileSize)
    {
        GameObject go = Instantiate(enemyPrefab);
        EnemyController enemy = go.GetComponent<EnemyController>();
        enemy.gridPosition = pos;
        enemy.Init(BoardManager.Instance, BoardManager.Instance.lantern);
        BoardManager.Instance.enemies.Add(enemy);
    }

    private void SpawnLantern(Vector2Int pos, float tileSize)
    {
        GameObject go = Instantiate(lanternPrefab);
        Lantern lantern = go.GetComponent<Lantern>();
        lantern.gridPosition = pos;
        lantern.Init(tileSize);
        BoardManager.Instance.lantern = lantern;
    }

    private void SpawnObstacle(Vector2Int pos, float tileSize)
    {
        GameObject go = Instantiate(obstaclePrefab);
        ObstacleController obs = go.GetComponent<ObstacleController>();
        obs.Init(pos, tileSize);
        BoardManager.Instance.obstacles.Add(pos);
    }
}