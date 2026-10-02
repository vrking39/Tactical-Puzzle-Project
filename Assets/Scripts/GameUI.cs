using UnityEngine;

public class GameUI : MonoBehaviour
{
    public Lantern lantern;

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 22;
        style.normal.textColor = Color.white;

        GUILayout.BeginArea(new Rect(20, 20, 350, 150));

        if (lantern != null)
        {
            GUILayout.Label($"🔥 LANTERN HP: {lantern.currentHealth} / {lantern.maxHealth}", style);
        }

        if (TurnManager.Instance != null)
        {
            GUILayout.Label($"TURN: {TurnManager.Instance.currentTurn} / {TurnManager.Instance.targetSurviveTurns}", style);
            GUILayout.Label($"STATE: {TurnManager.Instance.currentState}", style);
            
            if (TurnManager.Instance.IsPlayerTurn())
            {
                GUILayout.Label("[Press SPACE to End Turn]", style);
            }
        }

        GUILayout.EndArea();
    }
}