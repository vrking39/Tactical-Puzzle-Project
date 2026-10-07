using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    public static HeroController SelectedHero { get; private set; }

    [Header("Hero Stats")]
    public int attackDamage = 1;
    public int moveRange = 1;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Grid Position")]
    [SerializeField] private Vector2Int startingGridPosition = new Vector2Int(0, 0);

    public Vector2Int gridPosition { get; private set; }
    private Vector3 targetWorldPosition;
    private bool isMoving = false;

    void Start()
    {
        gridPosition = startingGridPosition; // Set to inspector value on start
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
            }
        }
    }

    private void OnMouseDown()
    {
        if (TurnManager.Instance != null && !TurnManager.Instance.IsPlayerTurn()) return;
        Select();
    }

    public void Select()
    {
        SelectedHero = this;
        Debug.Log($"Selected Hero at {gridPosition}");
    }

    public void Deselect()
    {
        if (SelectedHero == this)
        {
            SelectedHero = null;
        }
    }

    public void OnTileClicked(Vector2Int clickedPos)
    {
        if (TurnManager.Instance != null && !TurnManager.Instance.IsPlayerTurn()) return;
        if (isMoving) return;

        Vector2Int delta = clickedPos - gridPosition;
        int distance = Mathf.Abs(delta.x) + Mathf.Abs(delta.y);

        // 1. Check if clicking an adjacent enemy to attack
        EnemyController targetEnemy = BoardManager.Instance.GetEnemyAt(clickedPos);
        if (targetEnemy != null && distance == 1)
        {
            Debug.Log("⚔️ Hero attacks enemy!");
            targetEnemy.TakeDamage(attackDamage);
            Deselect();
            TurnManager.Instance.EndPlayerTurn();
            return;
        }

        // 2. Check if clicking an empty valid adjacent tile to move
        if (distance <= moveRange && !BoardManager.Instance.IsCellOccupied(clickedPos))
        {
            gridPosition = clickedPos;
            targetWorldPosition = GetWorldPosition(gridPosition);
            isMoving = true;

            Deselect();
            TurnManager.Instance.EndPlayerTurn();
            return;
        }

        // 3. Clicked somewhere invalid -> Deselect
        Deselect();
    }

    Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        float tileSize = BoardManager.Instance != null ? BoardManager.Instance.tileSize : 1f;
        return new Vector3(gridPos.x * tileSize, gridPos.y * tileSize, -1f);
    }
}