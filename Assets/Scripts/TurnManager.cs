using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState
{
    PlayerTurn,
    EnemyTurn,
    GameWon,
    GameLost
}

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    [Header("State Settings")]
    public GameState currentState = GameState.PlayerTurn;
    public int currentTurn = 1;
    public int targetSurviveTurns = 3; // Goal: Survive 3 turns

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        // For testing/manual turn ending: Press SPACE during Player Turn
        if (currentState == GameState.PlayerTurn)
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                EndPlayerTurn();
            }
        }
    }

    public void EndPlayerTurn()
    {
        if (currentState != GameState.PlayerTurn) return;

        Debug.Log($"--- END OF PLAYER TURN {currentTurn} ---");
        currentState = GameState.EnemyTurn;

        // Trigger Enemy Phase
        StartCoroutine(ExecuteEnemyTurn());
    }

    private System.Collections.IEnumerator ExecuteEnemyTurn()
    {
        Debug.Log("--- ENEMY TURN START ---");

        // Pause briefly so the enemy turn feels distinct
        yield return new WaitForSeconds(0.5f);

        // Enemy actions will happen here in Step 6 & 7

        yield return new WaitForSeconds(0.5f);

        // Turn complete, progress turn count
        currentTurn++;
        Debug.Log($"--- ENEMY TURN END | ADVANCING TO TURN {currentTurn} ---");

        currentState = GameState.PlayerTurn;
    }

    public bool IsPlayerTurn()
    {
        return currentState == GameState.PlayerTurn;
    }
}