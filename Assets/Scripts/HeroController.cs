using UnityEngine;

public class HeroController : MonoBehaviour
{
    public static HeroController SelectedHero { get; private set; }

    [Header("Hero Stats")]
    public int attackDamage = 1;
    public int moveRange = 1;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    public Vector2Int gridPosition { get; private set; } = new Vector2Int(0, 0);
    private Vector3 targetWorldPosition;
    private bool isMoving = false;

    void Start()
    {
        transform.position = GetWorldPosition(gridPosition);
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                targetWorldPosition, 
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetWorldPosition) < 0.001f)
            {
                transform.position = targetWorldPosition;
                isMoving = false;
                
                // Clear highlights when movement finishes
                if (BoardManager.Instance != null)
                {
                    BoardManager.Instance.ResetAllTileHighlights();
                }
            }
        }
    }

    public void InitPosition(Vector2Int pos, float tileSize)
    {
        gridPosition = pos;
        transform.position = GetWorldPosition(gridPosition);
    }

    private void OnMouseDown()
    {
        if (TurnManager.Instance != null && !TurnManager.Instance.IsPlayerTurn()) return;
        Select();
    }

    public void Select()
    {
        // Clear old highlights if switching heroes
        if (BoardManager.Instance != null)
        {
            BoardManager.Instance.ResetAllTileHighlights();
        }

        SelectedHero = this;
        Debug.Log($"Selected Hero at {gridPosition}");
        HighlightValidTiles();
    }

    public void Deselect()
    {
        if (SelectedHero == this)
        {
            SelectedHero = null;
        }

        if (BoardManager.Instance != null)
        {
            BoardManager.Instance.ResetAllTileHighlights();
        }
    }

    private void HighlightValidTiles()
    {
        if (BoardManager.Instance == null) return;

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int checkPos = gridPosition + dir;
            TileController tile = BoardManager.Instance.GetTileAt(checkPos);

            if (tile == null) continue;

            EnemyController enemy = BoardManager.Instance.GetEnemyAt(checkPos);
            if (enemy != null)
            {
                // Highlight Red for attack targets
                tile.Highlight(Color.red);
            }
            else if (!BoardManager.Instance.IsCellOccupied(checkPos))
            {
                // Highlight Cyan for empty walkable tiles
                tile.Highlight(Color.cyan);
            }
        }
    }

    public void OnTileClicked(Vector2Int clickedPos)
    {
        if (TurnManager.Instance != null && !TurnManager.Instance.IsPlayerTurn()) return;
        if (isMoving) return;

        Vector2Int delta = clickedPos - gridPosition;
        int distance = Mathf.Abs(delta.x) + Mathf.Abs(delta.y);

        // 1. Attack adjacent enemy
        EnemyController targetEnemy = BoardManager.Instance != null ? BoardManager.Instance.GetEnemyAt(clickedPos) : null;
        if (targetEnemy != null && distance == 1)
        {
            Debug.Log("⚔️ Hero attacks enemy!");
            
            UnitJuice juice = GetComponent<UnitJuice>();
            if (juice != null) juice.PunchTowards(targetEnemy.transform.position);

            targetEnemy.TakeDamage(attackDamage);
            Deselect();
            
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.EndPlayerTurn();
            }
            return;
        }

        // 2. Move to empty valid tile
        if (distance <= moveRange && BoardManager.Instance != null && !BoardManager.Instance.IsCellOccupied(clickedPos))
        {
            gridPosition = clickedPos;
            targetWorldPosition = GetWorldPosition(gridPosition);
            isMoving = true;

            Deselect();

            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.EndPlayerTurn();
            }
            return;
        }

        // 3. Invalid tile clicked -> Deselect
        Deselect();
    }

    Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        float tileSize = BoardManager.Instance != null ? BoardManager.Instance.tileSize : 1f;
        return new Vector3(gridPos.x * tileSize, gridPos.y * tileSize, -1f);
    }
}