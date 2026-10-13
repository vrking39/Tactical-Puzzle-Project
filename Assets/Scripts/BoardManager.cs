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
    public List<Vector2Int> obstacles = new List<Vector2Int>();

    private Dictionary<Vector2Int, TileController> tileMap = new Dictionary<Vector2Int, TileController>();

    private void Awake()
    {
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

        if (PuzzleLoader.Instance != null)
        {
            PuzzleLoader.Instance.LoadLevel(0);
        }
        else
        {
            Debug.LogError("PuzzleLoader Instance not found in the scene! Ensure PuzzleLoader script is attached to a GameObject.");
        }
    }

    void CreateBoard()
    {
        tileMap.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                tile.name = $"Tile_{x}_{y}";
                tile.transform.position = new Vector3(x * tileSize, y * tileSize, 0);
                tile.transform.localScale = Vector3.one * (tileSize * 0.9f);
                tile.transform.SetParent(transform);

                if (tile.GetComponent<Collider2D>() == null)
                {
                    tile.AddComponent<BoxCollider2D>();
                }

                TileController tileScript = tile.AddComponent<TileController>();
                tileScript.gridPosition = new Vector2Int(x, y);

                tileMap[tileScript.gridPosition] = tileScript;
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

    public EnemyController GetEnemyAt(Vector2Int gridPos)
    {
        return enemies.Find(e => e != null && !e.isDead && e.gridPosition == gridPos);
    }

    public HeroController GetHeroAt(Vector2Int gridPos)
    {
        return heroes.Find(h => h != null && h.gridPosition == gridPos);
    }

    public TileController GetTileAt(Vector2Int gridPos)
    {
        if (tileMap.TryGetValue(gridPos, out TileController tile))
        {
            return tile;
        }
        return null;
    }

    public void ResetAllTileHighlights()
    {
        foreach (var tile in tileMap.Values)
        {
            if (tile != null) tile.ResetColor();
        }
    }
}