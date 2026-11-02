using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneInspector
{
    public static void Inspect()
    {
        string scenePath = "Assets/Scenes/SampleScene.unity";
        Scene scene = EditorSceneManager.OpenScene(scenePath);

        Debug.Log("--- SCENE INSPECTION START ---");

        // Player
        PlayerMovement[] players = Object.FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None);
        Debug.Log("Players found: " + players.Length);
        foreach (var p in players) {
            Rigidbody2D rb = p.GetComponent<Rigidbody2D>();
            Debug.Log($"Player: {p.name}, Rigidbody2D: {rb != null}, Constraints: {(rb != null ? rb.constraints.ToString() : "N/A")}");
        }

        // Fruits
        Fruit[] fruits = Object.FindObjectsByType<Fruit>(FindObjectsSortMode.None);
        Debug.Log("Fruits found: " + fruits.Length);
        foreach (var f in fruits) {
            Animator anim = f.GetComponent<Animator>();
            SpriteRenderer sr = f.GetComponent<SpriteRenderer>();
            Debug.Log($"Fruit: {f.name}, Animator: {anim != null}, Sprite: {(sr != null && sr.sprite != null ? sr.sprite.name : "None")}");
        }

        // UI
        ScoreManager sm = Object.FindAnyObjectByType<ScoreManager>();
        Debug.Log($"ScoreManager: {sm != null}");
        if (sm != null) {
            Debug.Log($"ScoreManager scoreText assigned: {sm.scoreText != null}");
            Debug.Log($"ScoreManager gameOverText assigned: {sm.gameOverText != null}");
        }
        
        TMP_Text[] texts = Object.FindObjectsByType<TMP_Text>(FindObjectsSortMode.None);
        foreach (var t in texts) {
            Debug.Log($"TMP_Text: {t.name}, text: {t.text}");
        }

        // Other SpriteRenderers (for Background and Obstacles)
        SpriteRenderer[] srs = Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (var sr in srs) {
            if (sr.GetComponent<Fruit>() != null || sr.GetComponent<PlayerMovement>() != null) continue;
            Debug.Log($"SpriteRenderer: {sr.name}, Sprite: {(sr.sprite != null ? sr.sprite.name : "None")}, Layer: {sr.sortingLayerName}, Order: {sr.sortingOrder}");
            Collider2D col = sr.GetComponent<Collider2D>();
            Debug.Log($"   -> Collider: {col != null}");
        }

        Debug.Log("--- SCENE INSPECTION END ---");
    }
}
