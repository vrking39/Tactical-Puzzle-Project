using System.Collections;
using UnityEngine;

public class UnitJuice : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    /// <summary>
    /// Flashes the unit red briefly when it takes damage.
    /// </summary>
    public void FlashRed()
    {
        StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.12f);
        spriteRenderer.color = originalColor;
    }

    /// <summary>
    /// Briefly lurches the unit towards a target position to simulate a physical hit/attack.
    /// </summary>
    /// <param name="targetWorldPos">The position of the tile or unit being attacked.</param>
    public void PunchTowards(Vector3 targetWorldPos)
    {
        StartCoroutine(PunchRoutine(targetWorldPos));
    }

    private IEnumerator PunchRoutine(Vector3 targetWorldPos)
    {
        Vector3 startPos = transform.position;
        // Calculate a point 35% of the distance toward the target
        Vector3 peakPos = Vector3.Lerp(startPos, targetWorldPos, 0.35f);

        // Move forward quickly
        float elapsed = 0f;
        float duration = 0.08f;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, peakPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap back to starting position
        elapsed = 0f;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(peakPos, startPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = startPos;
    }
}