using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using Random = UnityEngine.Random;

public class Level : MonoBehaviour {
    #region Fields

    [SerializeField]
    SpriteRenderer spriteRenderer;

    [Header("Enemies"), SerializeField]
    GameObject enemyPrefab;

    [SerializeField]
    Transform enemyParent;

    [Header("Gaps"), SerializeField]
    GameObject gapPrefab;

    [SerializeField]
    Transform gapParent;

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
        // set random color
        spriteRenderer.color = GetLevelColor();

        CreateGaps();
        CreateEnemies();

        // set random rotation
        var euler = transform.eulerAngles;
        euler.z = Random.Range(0f, 360f);
        transform.eulerAngles = euler;
    }

    public void ShowLevel () {
        gameObject.SetActive(true);
        gameObject.transform.SetParent(levelManager.transform);
        IsActive = true;
    }

    public void HideLevel () {
        gameObject.SetActive(false);
        gameObject.transform.SetParent(levelManager.LevelPool);
        IsActive = false;
    }

    Color GetLevelColor () {
        var colors = new List<Color>(levelManager.LevelColors.Length);
        colors.AddRange(levelManager.LevelColors);
        return colors[Random.Range(0, levelManager.LevelColors.Length)];
    }

    void CreateGaps () {
        // don't create gaps if it's the first level
        if (levelManager.Levels.Count == 0) {
            return;
        }

        // get random amount of gaps to spawn
        var amount = Random.Range(3, 7);

        var unusedRotations = GetGapRoationList();

        for (var i = 0; i < amount; i++) {
            var newRotation = GetRandomGapRoation(unusedRotations);

            var go = Instantiate(gapPrefab);
            go.transform.SetParent(gapParent);
            go.transform.localScale = transform.localScale;
            go.name = "Gap " + (i + 1);

            // set random rotation
            var euler = go.transform.eulerAngles;
            euler.z = newRotation;
            go.transform.eulerAngles = euler;
        }
    }

    void CreateEnemies () {
        // don't create enemies if it's the first level
        if (levelManager.Levels.Count == 0) {
            return;
        }

        // get random amount of enemies to spawn
        var amount = Random.Range(0, 10);

        for (var i = 0; i < amount; i++) {
            var go = Instantiate(enemyPrefab).GetComponent<Enemy>();
            go.transform.SetParent(enemyParent);
            go.transform.localScale = transform.localScale;
            go.name = "Enemy " + (i + 1);

            // set random rotation
            var euler = go.transform.eulerAngles;
            euler.z = Random.Range(0f, 360f);
            go.transform.eulerAngles = euler;

            // check overlapping
            var collisions = Physics2D.OverlapCircleAll(go.EnemyTransform.position, go.Collider.radius);
            var loopSafety = 0;
            while (collisions.Length > 1 && loopSafety < 20) {
                euler.z += 20f;
                go.transform.eulerAngles = euler;
                collisions = Physics2D.OverlapCircleAll(go.EnemyTransform.position, go.Collider.radius);
                loopSafety++;
            }
        }
    }

    List<int> GetGapRoationList () {
        var rotations = new List<int>(8);
        for (var i = 0; i < 8; i++) {
            rotations.Add(i);
        }
        return rotations;
    }

    int GetRandomGapRoation (List<int> rotations) {
        var rotation = Random.Range(rotations[0], rotations.Count) * 45;
        rotations.Remove(rotation);
        return rotation;
    }

    #endregion
}