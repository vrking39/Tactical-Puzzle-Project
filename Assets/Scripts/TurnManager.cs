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

    [Header("References")]
    public EnemyController enemy;
    public Lantern lantern;

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
        // Allow restarting at any time with 'R'
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

        if (enemy != null)
        {
            yield return StartCoroutine(enemy.TakeTurn());
        }

        // Check if lantern was destroyed during enemy turn
        CheckGameOver();
        if (currentState == GameState.GameLost) yield break;

        // Check if survived required turns
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