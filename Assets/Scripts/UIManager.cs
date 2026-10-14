using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Text Elements")]
    public TextMeshProUGUI lanternHpText;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI stateText;

    [Header("Buttons / Overlay")]
    public GameObject nextLevelButton;
    public GameObject restartButton;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        UpdateHUD();
    }

    public void UpdateHUD()
    {
        // 1. Lantern HP Update
        Lantern lantern = BoardManager.Instance != null ? BoardManager.Instance.lantern : null;
        if (lantern != null && lanternHpText != null)
        {
            lanternHpText.text = $"🔥 Lantern HP: {lantern.currentHealth} / {lantern.maxHealth}";
        }

        // 2. Turn & Level Counters
        if (TurnManager.Instance != null)
        {
            if (turnText != null)
                turnText.text = $"Turn: {TurnManager.Instance.currentTurn} / {TurnManager.Instance.targetSurviveTurns}";

            if (levelText != null && PuzzleLoader.Instance != null)
                levelText.text = $"Level {PuzzleLoader.Instance.currentLevelIndex + 1}";

            // 3. State & Button Visibility
            switch (TurnManager.Instance.currentState)
            {
                case GameState.PlayerTurn:
                    if (stateText != null) stateText.text = "YOUR TURN (Click Hero to Move/Attack)";
                    if (nextLevelButton != null) nextLevelButton.SetActive(false);
                    if (restartButton != null) restartButton.SetActive(false);
                    break;

                case GameState.EnemyTurn:
                    if (stateText != null) stateText.text = "ENEMY PHASE...";
                    break;

                case GameState.GameWon:
                    if (stateText != null) stateText.text = "🎉 VICTORY! STAGE CLEARED!";
                    if (nextLevelButton != null) nextLevelButton.SetActive(true);
                    break;

                case GameState.GameLost:
                    if (stateText != null) stateText.text = "☠️ GAME OVER! THE LANTERN BROKE";
                    if (restartButton != null) restartButton.SetActive(true);
                    break;
            }
        }
    }

    // OnClick hook for Next Level UI Button
    public void OnNextLevelPressed()
    {
        if (PuzzleLoader.Instance != null && TurnManager.Instance != null)
        {
            PuzzleLoader.Instance.LoadNextLevel();
            TurnManager.Instance.currentTurn = 1;
            TurnManager.Instance.currentState = GameState.PlayerTurn;
        }
    }

    // OnClick hook for Restart UI Button
    public void OnRestartPressed()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.RestartGame();
        }
    }
}