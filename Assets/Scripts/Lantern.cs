using UnityEngine;

public class Lantern : MonoBehaviour
{
    [Header("Lantern Stats")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("Grid Position")]
    public Vector2Int gridPosition = new Vector2Int(3, 3);

    private float tileSize = 1f;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Init(float size)
    {
        tileSize = size;
        UpdatePosition();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        Debug.Log($"Lantern took damage! HP remaining: {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log("Lantern destroyed! GAME OVER");
            // Direct state check so loss happens instantly
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.CheckGameOver();
            }
        }
    }

    private void UpdatePosition()
    {
        transform.position = new Vector3(
            gridPosition.x * tileSize,
            gridPosition.y * tileSize,
            -0.5f
        );
    }
}