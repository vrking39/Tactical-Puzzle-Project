using UnityEngine;

public class GameUI : MonoBehaviour
{
    public Lantern lantern;

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 22;
        style.normal.textColor = Color.white;

        GUILayout.BeginArea(new Rect(20, 20, 450, 250));

        if (lantern != null)
        {
            GUILayout.Label($"🔥 LANTERN HP: {lantern.currentHealth} / {lantern.maxHealth}", style);
        }

        if (TurnManager.Instance != null)
        {
            GUILayout.Label($"TURN: {TurnManager.Instance.currentTurn} / {TurnManager.Instance.targetSurviveTurns}", style);
            
            if (TurnManager.Instance.currentState == GameState.PlayerTurn)
            {
                GUILayout.Label("Mouse Click to Move | [F] Attack Adjacent | [SPACE] Pass Turn", style);
            }
            else if (TurnManager.Instance.currentState == GameState.GameWon)
            {
                style.normal.textColor = Color.green;
                GUILayout.Label("🎉 YOU SURVIVED! YOU WIN!", style);
                style.normal.textColor = Color.white;
                GUILayout.Label("[Press R to Restart]", style);
            }
            else if (TurnManager.Instance.currentState == GameState.GameLost)
            {
                style.normal.textColor = Color.red;
                GUILayout.Label("☠️ THE LANTERN DIED! GAME OVER", style);
                style.normal.textColor = Color.white;
                GUILayout.Label("[Press R to Restart]", style);
            }
        }

        GUILayout.EndArea();
    }
}