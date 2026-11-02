using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class SceneFixer
{
    [MenuItem("Tools/Fix Scene")]
    public static void Fix()
    {
        string scenePath = "Assets/Scenes/SampleScene.unity";
        Scene scene = EditorSceneManager.OpenScene(scenePath);

        Debug.Log("Fixing Scene...");

        // 1. Fix Player
        PlayerMovement[] players = Object.FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None);
        if (players.Length > 0)
        {
            PlayerMovement player = players[0];
            player.gameObject.tag = "Player";
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            // Delete extra players
            for (int i = 1; i < players.Length; i++)
            {
                Object.DestroyImmediate(players[i].gameObject);
            }
        }

        // 2. Fix Fruits
        Fruit[] fruits = Object.FindObjectsByType<Fruit>(FindObjectsSortMode.None);
        
        // Remove Animators from fruits so they don't change sprites
        foreach (Fruit f in fruits)
        {
            Animator anim = f.GetComponent<Animator>();
            if (anim != null)
            {
                Object.DestroyImmediate(anim);
            }
            Collider2D col = f.GetComponent<Collider2D>();
            if (col != null) col.isTrigger = true;
        }

        // Ensure EXACTLY 5 fruits
        if (fruits.Length > 5)
        {
            for (int i = 5; i < fruits.Length; i++) Object.DestroyImmediate(fruits[i].gameObject);
        }
        else if (fruits.Length < 5)
        {
            int toAdd = 5 - fruits.Length;
            GameObject template = fruits.Length > 0 ? fruits[0].gameObject : null;
            if (template != null)
            {
                for (int i = 0; i < toAdd; i++)
                {
                    GameObject newFruit = Object.Instantiate(template);
                    newFruit.name = "Fruit_" + (fruits.Length + i);
                    newFruit.transform.position = template.transform.position + new Vector3((i + 1) * 2, 0, 0);
                }
            }
        }
        
        fruits = Object.FindObjectsByType<Fruit>(FindObjectsSortMode.None); // Refresh list

        // Assign different sprites
        string[] fruitGuids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/Pixel Adventure 1/Assets/Items/Fruits" });
        if (fruitGuids.Length > 0 && fruits.Length > 0)
        {
            for (int i = 0; i < fruits.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(fruitGuids[i % fruitGuids.Length]);
                Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (s != null)
                {
                    SpriteRenderer sr = fruits[i].GetComponent<SpriteRenderer>();
                    if (sr != null) sr.sprite = s;
                }
            }
        }

        // 3. Fix Obstacles
        GameObject[] obstacles = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
            .Where(g => g.name.ToLower().Contains("obstacle") || g.name.ToLower().Contains("spike") || g.CompareTag("Obstacle")).ToArray();

        if (obstacles.Length > 5)
        {
            for (int i = 5; i < obstacles.Length; i++) Object.DestroyImmediate(obstacles[i]);
        }
        else if (obstacles.Length < 5)
        {
            int toAdd = 5 - obstacles.Length;
            GameObject template = obstacles.Length > 0 ? obstacles[0] : null;
            
            string[] spikeGuids = AssetDatabase.FindAssets("t:Sprite Spike");
            Sprite spikeSprite = spikeGuids.Length > 0 ? AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(spikeGuids[0])) : null;

            for (int i = 0; i < toAdd; i++)
            {
                GameObject obs = null;
                if (template != null)
                {
                    obs = Object.Instantiate(template);
                }
                else
                {
                    obs = new GameObject("Obstacle");
                    SpriteRenderer sr = obs.AddComponent<SpriteRenderer>();
                    if (spikeSprite != null) sr.sprite = spikeSprite;
                    obs.AddComponent<BoxCollider2D>();
                    obs.tag = "Obstacle";
                }
                obs.transform.position = new Vector3(10 + i * 5, 0, 0);
            }
        }

        // Ensure Obstacles have colliders and no animators
        obstacles = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
            .Where(g => g.name.ToLower().Contains("obstacle") || g.name.ToLower().Contains("spike") || g.CompareTag("Obstacle")).ToArray();
        
        foreach (var obs in obstacles)
        {
            Animator anim = obs.GetComponent<Animator>();
            if (anim != null) Object.DestroyImmediate(anim);
            
            if (obs.GetComponent<Collider2D>() == null)
            {
                obs.AddComponent<BoxCollider2D>();
            }
        }

        // 4. Background
        SpriteRenderer[] srs = Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        bool hasBg = false;
        foreach (SpriteRenderer sr in srs)
        {
            if (sr.gameObject.name.ToLower().Contains("bg") || sr.gameObject.name.ToLower().Contains("background"))
            {
                sr.sortingOrder = -100;
                hasBg = true;
                
                // Ensure it has a nice sprite if empty
                if (sr.sprite == null) {
                    string bgPath = "Assets/Jovial Games/Basic 2D Side Scroller Art Kit/Sprites/Background/Sky_Background.png";
                    sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(bgPath);
                    sr.transform.localScale = new Vector3(3, 3, 1);
                }
            }
        }

        if (!hasBg)
        {
            GameObject bgGo = new GameObject("Background");
            SpriteRenderer bgSr = bgGo.AddComponent<SpriteRenderer>();
            bgSr.sortingOrder = -100;
            string bgPath = "Assets/Jovial Games/Basic 2D Side Scroller Art Kit/Sprites/Background/Sky_Background.png";
            Sprite bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(bgPath);
            if (bgSprite != null)
            {
                bgSr.sprite = bgSprite;
                bgGo.transform.localScale = new Vector3(3, 3, 1);
            }
        }

        // 5. UI and ScoreManager
        ScoreManager sm = Object.FindAnyObjectByType<ScoreManager>();
        if (sm == null)
        {
            GameObject smGo = new GameObject("ScoreManager");
            sm = smGo.AddComponent<ScoreManager>();
        }

        TMP_Text scoreText = Object.FindObjectsByType<TMP_Text>(FindObjectsSortMode.None).FirstOrDefault(t => t.name.ToLower().Contains("score"));
        GameObject gameOverText = Object.FindObjectsByType<TMP_Text>(FindObjectsSortMode.None).FirstOrDefault(t => t.name.ToLower().Contains("game") || t.text.ToLower().Contains("game over"))?.gameObject;

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGo = new GameObject("Canvas");
            canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        if (scoreText == null)
        {
            GameObject textGo = new GameObject("ScoreText");
            textGo.transform.SetParent(canvas.transform, false);
            scoreText = textGo.AddComponent<TextMeshProUGUI>();
            scoreText.text = "Score: 0";
            RectTransform rt = textGo.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(50, -50);
            scoreText.fontSize = 36;
        }

        if (gameOverText == null)
        {
            GameObject goText = new GameObject("GameOverText");
            goText.transform.SetParent(canvas.transform, false);
            TMP_Text go = goText.AddComponent<TextMeshProUGUI>();
            go.text = "GAME OVER";
            go.fontSize = 72;
            go.alignment = TextAlignmentOptions.Center;
            RectTransform rt = goText.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            gameOverText = goText;
        }

        sm.scoreText = scoreText;
        sm.gameOverText = gameOverText;
        gameOverText.SetActive(false);

        EditorSceneManager.SaveScene(scene);
        Debug.Log("Scene fixing complete and saved.");
    }
}
