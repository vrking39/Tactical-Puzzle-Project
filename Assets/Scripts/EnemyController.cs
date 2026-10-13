using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int maxHealth = 1;
    public int currentHealth;
    public int moveRange = 1;
    public bool isDead { get; private set; } = false;

    [Header("Position & Movement")]
    public Vector2Int gridPosition = new Vector2Int(5, 5);
    public float moveSpeed = 5f;

    private BoardManager boardManager;
    private Lantern targetLantern;
    private float tileSize = 1f;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Init(BoardManager board, Lantern lantern)
    {
        boardManager = board;
        targetLantern = lantern;
        tileSize = boardManager.tileSize;

        transform.position = GetWorldPosition(gridPosition);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage! HP left: {currentHealth}");

        UnitJuice juice = GetComponent<UnitJuice>();
        if (juice != null) juice.FlashRed();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Enemy defeated!");

        Renderer rend = GetComponent<Renderer>();
        if (rend != null) rend.enabled = false;

        gridPosition = new Vector2Int(-999, -999);
    }

    public IEnumerator TakeTurn()
    {
        if (isDead || currentHealth <= 0 || boardManager == null) yield break;

        // Choose target: Lantern or Hero (whichever is closer)
        Vector2Int targetPos = targetLantern != null ? targetLantern.gridPosition : gridPosition;
        HeroController hero = boardManager.heroes.Count > 0 ? boardManager.heroes[0] : null;

        if (hero != null && targetLantern != null)
        {
            int distToLantern = Mathf.Abs(targetLantern.gridPosition.x - gridPosition.x) + Mathf.Abs(targetLantern.gridPosition.y - gridPosition.y);
            int distToHero = Mathf.Abs(hero.gridPosition.x - gridPosition.x) + Mathf.Abs(hero.gridPosition.y - gridPosition.y);

            if (distToHero < distToLantern)
            {
                targetPos = hero.gridPosition;
            }
        }

        Vector2Int delta = targetPos - gridPosition;

        // Attack if adjacent to Lantern
        if (targetLantern != null && Mathf.Abs(targetLantern.gridPosition.x - gridPosition.x) + Mathf.Abs(targetLantern.gridPosition.y - gridPosition.y) <= 1)
        {
            Debug.Log("💥 Enemy attacks the Lantern!");
            
            UnitJuice juice = GetComponent<UnitJuice>();
            if (juice != null) juice.PunchTowards(targetLantern.transform.position);

            targetLantern.TakeDamage(1);
            yield return new WaitForSeconds(0.3f);
            yield break;
        }

        // Move toward chosen target
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
    }

    Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * tileSize, gridPos.y * tileSize, -0.5f);
    }
}