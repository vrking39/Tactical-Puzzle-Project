using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int width = 6;
    public int height = 6;
    public float tileSize = 1f;

    [Header("Entities")]
    public Lantern lantern;
    public HeroController hero;

    void Start()
    {
        CreateBoard();

        // Initialize lantern size/position if assigned
        if (lantern != null)
        {
            lantern.Init(tileSize);
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
        // Check if lantern is on this tile
        if (lantern != null && lantern.gridPosition == gridPos)
            return true;

        return false;
    }
}