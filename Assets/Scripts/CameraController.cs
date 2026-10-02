using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Target Board")]
    public BoardManager boardManager;

    [Header("Padding Settings")]
    public float padding = 1.5f; // Extra border space around the grid

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        if (boardManager != null)
        {
            CenterAndFitBoard();
        }
    }

    public void CenterAndFitBoard()
    {
        float tileSize = boardManager.tileSize;
        int width = boardManager.width;
        int height = boardManager.height;

        // 1. Calculate world center of the generated grid
        float centerX = (width - 1) * tileSize / 2f;
        float centerY = (height - 1) * tileSize / 2f;
        transform.position = new Vector3(centerX, centerY, -10f); // -10 on Z axis for 2D camera

        // 2. Adjust orthographic size to fit the board within screen aspect ratio
        float boardWidth = width * tileSize;
        float boardHeight = height * tileSize;

        float screenAspect = (float)Screen.width / Screen.height;
        float targetAspect = boardWidth / boardHeight;

        if (screenAspect >= targetAspect)
        {
            // Screen is wider than the board -> fit height
            cam.orthographicSize = (boardHeight / 2f) * padding;
        }
        else
        {
            // Screen is taller/narrower than the board -> fit width
            float differenceInSize = targetAspect / screenAspect;
            cam.orthographicSize = (boardHeight / 2f) * differenceInSize * padding;
        }
    }
}