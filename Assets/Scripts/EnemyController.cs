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
        if (targetLantern == null || boardManager == null) yield break;

        Vector2Int lanternPos = targetLantern.gridPosition;
        Vector2Int delta = lanternPos - gridPosition;

        // Check if adjacent to Lantern (Manhattan distance == 1)
        if (Mathf.Abs(delta.x) + Mathf.Abs(delta.y) <= 1)
        {
            Debug.Log("💥 Enemy attacks the Lantern!");
            targetLantern.TakeDamage(1);
            yield return new WaitForSeconds(0.3f);
            yield break;
        }

        // Decide movement direction toward Lantern
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

        // Move if target tile is free
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
    }

    Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * tileSize, gridPos.y * tileSize, -0.5f);
    }
}