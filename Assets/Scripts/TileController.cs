using UnityEngine;

public class TileController : MonoBehaviour
{
    public Vector2Int gridPosition;

    private SpriteRenderer spriteRenderer;
    private Color defaultColor = Color.white;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            defaultColor = spriteRenderer.color;
        }
    }

    private void OnMouseDown()
    {
        HeroController selectedHero = HeroController.SelectedHero;

        if (selectedHero != null)
        {
            selectedHero.OnTileClicked(gridPosition);
        }
        else
        {
            HeroController clickedHero = BoardManager.Instance != null ? BoardManager.Instance.GetHeroAt(gridPosition) : null;
            if (clickedHero != null)
            {
                clickedHero.Select();
            }
        }
    }

    public void Highlight(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }

    public void ResetColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = defaultColor;
        }
    }
}