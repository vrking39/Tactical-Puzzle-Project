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

    [Header("Level Definition (Bottom-to-Top Y index)")]
    // Row 0 is the bottom row (y=0), Row 5 is the top row (y=5)
    public string[] levelLayout = new string[]
    {
        ". . . H . .", // Row 0 (Y=0)
        ". X . . . .", // Row 1 (Y=1)
        ". . . L . .", // Row 2 (Y=2)
        ". . . . . .", // Row 3 (Y=3)
        ". . X X . .", // Row 4 (Y=4)
        "E . . . . E"  // Row 5 (Y=5)
    };

    private void Awake()
    {
        Instance = this;
    }

    public void LoadLevel()
    {
        BoardManager board = BoardManager.Instance;
        if (board == null) return;

        float tileSize = board.tileSize;

        for (int y = 0; y < levelLayout.Length; y++)
        {
            string[] tokens = levelLayout[y].Split(' ');

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