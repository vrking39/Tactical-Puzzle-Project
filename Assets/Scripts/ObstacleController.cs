using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public Vector2Int gridPosition;

    public void Init(Vector2Int pos, float tileSize)
    {
        gridPosition = pos;
        transform.position = new Vector3(pos.x * tileSize, pos.y * tileSize, -0.5f);
    }
}