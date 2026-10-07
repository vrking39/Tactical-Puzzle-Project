using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    [Header("Board Dimensions")]
    public int width = 6;
    public int height = 6;
    public float tileSize = 1f;

    [Header("Entities")]
    public Lantern lantern;
    public List<HeroController> heroes = new List<HeroController>();
    public List<EnemyController> enemies = new List<EnemyController>();
    public List<Vector2Int> obstacles = new List<Vector2Int>(); // For future walls/rocks

    private void Awake()
    {
        // Singleton pattern so mouse scripts and AI can query the board easily
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        CreateBoard();

        if (lantern != null)
        {
            lantern.Init(tileSize);
        }

        // Initialize all active enemies
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.Init(this, lantern);
            }
        }
    }

    void CreateBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                tile.name = $"Tile_{x}_{y}";
                tile.transform.position = new Vector3(x * tileSize, y * tileSize, 0);
                tile.transform.localScale = Vector3.one * (tileSize * 0.9f);
                tile.transform.SetParent(transform);

                // Add a BoxCollider2D so mouse clicks can detect this tile
                if (tile.GetComponent<Collider2D>() == null)
                {
                    tile.AddComponent<BoxCollider2D>();
                }

                // Attach a script to store tile coordinates for mouse interaction
                TileController tileScript = tile.AddComponent<TileController>();
                tileScript.gridPosition = new Vector2Int(x, y);
            }
        }
    }

    public bool IsCellOccupied(Vector2Int gridPos)
    {
        if (lantern != null && lantern.gridPosition == gridPos) return true;
        if (obstacles.Contains(gridPos)) return true;

        foreach (var hero in heroes)
        {
            if (hero != null && hero.gridPosition == gridPos) return true;
        }

        foreach (var enemy in enemies)
        {
            if (enemy != null && !enemy.isDead && enemy.gridPosition == gridPos) return true;
        }

        return false;
    }

    // Helper method to find an enemy on a specific grid tile
    public EnemyController GetEnemyAt(Vector2Int gridPos)
    {
        return enemies.Find(e => e != null && !e.isDead && e.gridPosition == gridPos);
    }

    // Helper method to find a hero on a specific grid tile
    public HeroController GetHeroAt(Vector2Int gridPos)
    {
        return heroes.Find(h => h != null && h.gridPosition == gridPos);
    }
}