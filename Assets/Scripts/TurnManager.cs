using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    public int targetSurviveTurns = 3;

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
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            RestartGame();
        }

        if (currentState == GameState.PlayerTurn)
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                EndPlayerTurn();
            }
        }
    }

    public void CheckGameOver()
    {
        Lantern lantern = BoardManager.Instance != null ? BoardManager.Instance.lantern : null;
        if (lantern != null && lantern.currentHealth <= 0)
        {
            currentState = GameState.GameLost;
            Debug.Log("--- GAME OVER: YOU LOST ---");
        }
    }

    public void EndPlayerTurn()
    {
        if (currentState != GameState.PlayerTurn) return;

        currentState = GameState.EnemyTurn;
        StartCoroutine(ExecuteEnemyTurn());
    }

    private IEnumerator ExecuteEnemyTurn()
    {
        yield return new WaitForSeconds(0.2f);

        // Loop through all enemies in BoardManager sequentially
        if (BoardManager.Instance != null)
        {
            for (int i = 0; i < BoardManager.Instance.enemies.Count; i++)
            {
                EnemyController enemy = BoardManager.Instance.enemies[i];
                if (enemy != null && !enemy.isDead)
                {
                    yield return StartCoroutine(enemy.TakeTurn());
                    yield return new WaitForSeconds(0.1f);
                }
            }

            // Remove dead enemies from list after turn completion
            BoardManager.Instance.enemies.RemoveAll(e => e == null || e.isDead);
        }

        // 1. Check Win/Loss conditions
        CheckGameOver();
        if (currentState == GameState.GameLost) yield break;

        // Win Condition 1: All enemies are defeated
        if (BoardManager.Instance.enemies.Count == 0)
        {
            currentState = GameState.GameWon;
            Debug.Log("--- VICTORY: ALL ENEMIES DEFEATED! ---");
            yield break;
        }

        // Win Condition 2: Survived target turns
        if (currentTurn >= targetSurviveTurns)
        {
            currentState = GameState.GameWon;
            Debug.Log("--- VICTORY: YOU SURVIVED! ---");
            yield break;
        }

        yield return new WaitForSeconds(0.2f);

        currentTurn++;
        currentState = GameState.PlayerTurn;
    }

    public bool IsPlayerTurn()
    {
        return currentState == GameState.PlayerTurn;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}