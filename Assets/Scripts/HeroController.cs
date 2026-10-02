using UnityEngine;
using UnityEngine.InputSystem;

public class HeroController : MonoBehaviour
{
    [Header("Board Reference")]
    public BoardManager boardManager;

    [Header("Hero Stats")]
    public int attackDamage = 1;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    public Vector2Int gridPosition { get; private set; } = new Vector2Int(0, 0);
    private Vector3 targetWorldPosition;
    private bool isMoving = false;

    private float tileSize = 1f;
    private Vector2Int gridMin = Vector2Int.zero;
    private Vector2Int gridMax;

    void Start()
    {
        if (boardManager != null)
        {
            tileSize = boardManager.tileSize;
            gridMax = new Vector2Int(boardManager.width - 1, boardManager.height - 1);
        }

        targetWorldPosition = GetWorldPosition(gridPosition);
        transform.position = targetWorldPosition;
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
            }
            
            return;
        }

        HandleInput();
    }

    void HandleInput()
    {
        if (TurnManager.Instance != null && !TurnManager.Instance.IsPlayerTurn()) return;
        if (Keyboard.current == null) return;

        // Attack Check (Press 'F' to attack adjacent enemy)
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (TryAttack())
            {
                // Ending turn automatically after attacking
                TurnManager.Instance.EndPlayerTurn();
                return;
            }
        }

        // Movement Check
        Vector2Int direction = Vector2Int.zero;

        if (Keyboard.current.wKey.wasPressedThisFrame)
            direction = Vector2Int.up;
        else if (Keyboard.current.sKey.wasPressedThisFrame)
            direction = Vector2Int.down;
        else if (Keyboard.current.aKey.wasPressedThisFrame)
            direction = Vector2Int.left;
        else if (Keyboard.current.dKey.wasPressedThisFrame)
            direction = Vector2Int.right;

        if (direction != Vector2Int.zero)
        {
            TryMove(direction);
        }
    }

    bool TryAttack()
    {
        if (boardManager == null || boardManager.enemy == null) return false;

        EnemyController enemy = boardManager.enemy;
        Vector2Int enemyPos = enemy.gridPosition;
        Vector2Int delta = enemyPos - gridPosition;

        // Check if enemy is in an adjacent cardinal tile (distance == 1)
        if (Mathf.Abs(delta.x) + Mathf.Abs(delta.y) == 1)
        {
            Debug.Log("⚔️ Hero attacks the enemy!");
            enemy.TakeDamage(attackDamage);
            return true;
        }

        Debug.Log("No enemy in range to attack!");
        return false;
    }

    void TryMove(Vector2Int direction)
    {
        Vector2Int targetGridPos = gridPosition + direction;

        if (IsWithinBounds(targetGridPos) && !boardManager.IsCellOccupied(targetGridPos))
        {
            gridPosition = targetGridPos;
            targetWorldPosition = GetWorldPosition(gridPosition);
            isMoving = true;
        }
    }

    bool IsWithinBounds(Vector2Int targetPos)
    {
        return targetPos.x >= gridMin.x && targetPos.x <= gridMax.x &&
               targetPos.y >= gridMin.y && targetPos.y <= gridMax.y;
    }

    Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * tileSize, gridPos.y * tileSize, -1f);
    }
}