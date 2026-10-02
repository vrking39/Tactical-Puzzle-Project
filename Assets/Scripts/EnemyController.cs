using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Position & Movement")]
    public Vector2Int gridPosition = new Vector2Int(5, 5); // Default spawn corner
    public float moveSpeed = 5f;

    private BoardManager boardManager;
    private Lantern targetLantern;
    private float tileSize = 1f;

    public void Init(BoardManager board, Lantern lantern)
    {
        boardManager = board;
        targetLantern = lantern;
        tileSize = boardManager.tileSize;

        transform.position = GetWorldPosition(gridPosition);
    }

    // Called automatically by TurnManager during Enemy Turn
    public IEnumerator TakeTurn()
    {
        if (targetLantern == null) 
        {
            Debug.LogError("Enemy has no targetLantern assigned!");
            yield break;
        }

        if (boardManager == null)
        {
            Debug.LogError("Enemy has no boardManager assigned!");
            yield break;
        }

        Debug.Log($"Enemy taking turn from {gridPosition} towards {targetLantern.gridPosition}");

        Vector2Int lanternPos = targetLantern.gridPosition;
        Vector2Int delta = lanternPos - gridPosition;

        // Check if already adjacent (Distance = 1)
        if (Mathf.Abs(delta.x) + Mathf.Abs(delta.y) <= 1)
        {
            Debug.Log("Enemy is adjacent to Lantern! Cannot move closer.");
            yield break;
        }

        // Decide step direction
        Vector2Int moveDir = Vector2Int.zero;
        if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
        {
            moveDir.x = System.Math.Sign(delta.x);
        }
        else
        {
            moveDir.y = System.Math.Sign(delta.y);
        }

        Vector2Int targetGridPos = gridPosition + moveDir;

        if (!boardManager.IsCellOccupied(targetGridPos))
        {
            gridPosition = targetGridPos;
            Vector3 targetWorldPos = GetWorldPosition(gridPosition);

            while (Vector3.Distance(transform.position, targetWorldPos) > 0.001f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, 
                    targetWorldPos, 
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            transform.position = targetWorldPos;
        }
        else
        {
            Debug.Log($"Target cell {targetGridPos} is occupied! Enemy holds position.");
        }
    }

    Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * tileSize, gridPos.y * tileSize, -0.5f);
    }
}