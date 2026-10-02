using UnityEngine;
using UnityEngine.InputSystem;

public class HeroController : MonoBehaviour
{
    [Header("Board Reference")]
    public BoardManager boardManager; // Drag your BoardManager GameObject here in the Inspector

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Vector2Int gridPosition = new Vector2Int(0, 0);
    private Vector3 targetWorldPosition;
    private bool isMoving = false;

    private float tileSize = 1f;
    private Vector2Int gridMin = Vector2Int.zero;
    private Vector2Int gridMax;

    void Start()
    {
        // Automatically sync with BoardManager dimensions
        if (boardManager != null)
        {
            tileSize = boardManager.tileSize;
            // 0 to width - 1 (e.g. 0 to 5 for a width of 6)
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
        if (Keyboard.current == null) return;

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

    void TryMove(Vector2Int direction)
    {
        Vector2Int targetGridPos = gridPosition + direction;

        if (IsWithinBounds(targetGridPos))
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