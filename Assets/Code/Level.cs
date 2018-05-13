using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Level : MonoBehaviour {
    #region Fields

    [SerializeField]
    SpriteRenderer spriteRenderer;

    [SerializeField]
    Sprite[] levelSprites;

    [SerializeField]
    GameObject enemyPrefab;

    LevelManager levelManager;

    #endregion

    #region Properties

    public bool IsActive { get; private set; }

    #endregion

    #region Methods

    #region Unity

    void OnEnable () {
        levelManager = LevelManager.Instance;
    }

    #endregion

    public void Initialize () {
        // get random sprite
        spriteRenderer.sprite = GetLevelSprite();

        // set random color
        spriteRenderer.color = GetLevelColor();

        // set random rotation
        var euler = transform.eulerAngles;
        euler.z = Random.Range(0f, 360f);
        transform.eulerAngles = euler;

        CreateEnemies();
    }

    public void ShowLevel () {
        gameObject.SetActive(true);
        //spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        gameObject.transform.SetParent(levelManager.transform);
        IsActive = true;
    }

    public void HideLevel () {
        gameObject.SetActive(false);
        // spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.1f);
        gameObject.transform.SetParent(levelManager.LevelPool);
        IsActive = false;
    }

    Color GetLevelColor () {
        var colors = new List<Color>(levelManager.LevelColors.Length);
        colors.AddRange(levelManager.LevelColors);
        return colors[Random.Range(0, levelManager.LevelColors.Length)];
    }

    Sprite GetLevelSprite () {
        return levelSprites[Random.Range(0, levelSprites.Length)];
    }

    void CreateEnemies () {
        // get random amount of enemies to spawn
        var amount = Random.Range(0, 4);

        for (var i = 0; i < amount; i++) {
            var go = Instantiate(enemyPrefab);
            go.transform.SetParent(transform);
            go.transform.localScale = transform.localScale;

            // set random rotation
            var euler = go.transform.eulerAngles;
            euler.z = Random.Range(0f, 360f);
            go.transform.eulerAngles = euler;
        }
    }

    #endregion
}