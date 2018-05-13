using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTest : MonoBehaviour {
    [SerializeField]
    SpriteRenderer spriteRenderer;

    [SerializeField]
    Text index;

    Color[] colors = {Color.blue, Color.yellow, Color.red, Color.green, Color.cyan, Color.magenta};

    public float SpriteWidth {
        get { return spriteRenderer.sprite.bounds.size.x; }
    }

    public string Index {
        get { return index.text; }
        set { index.text = value; }
    }

    public void Set () {
        spriteRenderer.color = RandomColor();
    }

    public void Show () {
        // spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        gameObject.SetActive(true);
        gameObject.transform.SetParent(LevelManagerTest.Instance.transform);
    }

    public void Hide () {
        // spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.1f);
        gameObject.SetActive(false);
        gameObject.transform.SetParent(LevelManagerTest.Instance.LevelPool);
    }

    Color RandomColor () {
        return colors[Random.Range(0, colors.Length)];
    }
}