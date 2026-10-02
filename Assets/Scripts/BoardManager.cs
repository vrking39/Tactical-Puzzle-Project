using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int width = 6;
    public int height = 6;
    public float tileSize = 1f;

    [Header("Entities")]
    public Lantern lantern;
    public HeroController hero;
    public EnemyController enemy; // Drag your enemy instance here

    void Start()
    {
        CreateBoard();

        // 1. Initialize Lantern
        if (lantern != null)
        {
            lantern.Init(tileSize);
        }

        // 2. Initialize Enemy
        if (enemy != null && lantern != null)
        {
            enemy.Init(this, lantern);
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
            }
        }
    }

    // Helper method to check cell accessibility
    public bool IsCellOccupied(Vector2Int gridPos)
    {
        if (lantern != null && lantern.gridPosition == gridPos) return true;
        if (hero != null && hero.gridPosition == gridPos) return true;
        if (enemy != null && enemy.gridPosition == gridPos) return true;

        return false;
    }
}