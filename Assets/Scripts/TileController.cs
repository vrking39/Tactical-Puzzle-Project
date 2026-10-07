using UnityEngine;

public class TileController : MonoBehaviour
{
    public Vector2Int gridPosition;

    private void OnMouseDown()
    {
        // We will hook this up to unit movement/selection in the next step!
        Debug.Log($"Clicked tile at {gridPosition}");
    }
}