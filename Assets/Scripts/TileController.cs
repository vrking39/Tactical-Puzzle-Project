using UnityEngine;

public class TileController : MonoBehaviour
{
    public Vector2Int gridPosition;
    private SpriteRenderer spriteRenderer;

    // Visual indicators for selection
    private Color defaultColor = Color.white;
    private Color highlightColor = new Color(0.2f, 0.8f, 1f, 0.5f); // Light blue for valid move
    private Color attackColor = new Color(1f, 0.2f, 0.2f, 0.5f);    // Light red for attack target

    private bool isHighlightActive = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        // Pass click event to Hero selection logic
        HeroController selectedHero = HeroController.SelectedHero;

        if (selectedHero != null)
        {
            selectedHero.OnTileClicked(gridPosition);
        }
        else
        {
            // Check if clicking on a Hero to select it
            HeroController clickedHero = BoardManager.Instance.GetHeroAt(gridPosition);
            if (clickedHero != null)
            {
                clickedHero.Select();
            }
        }
    }

    public void SetHighlight(bool active, bool isAttack = false)
    {
        isHighlightActive = active;
        if (spriteRenderer == null) return;

        if (!active)
        {
            spriteRenderer.color = defaultColor;
        }
        else
        {
            spriteRenderer.color = isAttack ? attackColor : highlightColor;
        }
    }
}