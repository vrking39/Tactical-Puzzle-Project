using UnityEngine;

public class GameUI : MonoBehaviour
{
    public Lantern lantern;

    private void OnGUI()
    {
        if (lantern == null) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = Color.white;

        GUILayout.BeginArea(new Rect(20, 20, 300, 100));
        GUILayout.Label($"🔥 LANTERN HP: {lantern.currentHealth} / {lantern.maxHealth}", style);
        GUILayout.EndArea();
    }
}