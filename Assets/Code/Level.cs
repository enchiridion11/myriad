﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Level : MonoBehaviour {
    #region Fields

    [SerializeField]
    SpriteRenderer spriteRenderer;

    [SerializeField]
    PolygonCollider2D collider;

    [Header ("Enemies"), SerializeField]
    GameObject enemyPrefab;

    [SerializeField]
    Transform enemyParent;

    [Header ("Gaps"), SerializeField]
    GameObject gapPrefab;

    [SerializeField]
    Transform gapParent;

    [Header ("Collectables"), SerializeField]
    GameObject collectablePrefab;

    [SerializeField]
    Transform collectableParent;

    LevelManager levelManager;

    #endregion

    #region Properties

    public bool IsActive { get; private set; }
    
    public int LevelNumber { get; set; }

    #endregion

    #region Methods

    #region Unity

    void OnEnable () {
        levelManager = LevelManager.Instance;
    }

    #endregion

    public void Initialize () {
        // set random color
        spriteRenderer.color = GetLevelColor ();

        CreateGaps ();
        CreateCollectables ();
        CreateEnemies ();

        // set random rotation
        var euler = transform.eulerAngles;
        euler.z = Random.Range (0f, 360f);
        transform.eulerAngles = euler;
    }

    public void ShowLevel () {
        gameObject.SetActive (true);
        //spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        gameObject.transform.SetParent (levelManager.transform);
        IsActive = true;
    }

    public void HideLevel () {
        gameObject.SetActive (false);
        // spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.1f);
        gameObject.transform.SetParent (levelManager.LevelPool);
        IsActive = false;
    }

    Color GetLevelColor () {
        var colors = new List<Color> (levelManager.LevelColors.Length);
        colors.AddRange (levelManager.LevelColors);
        return colors[Random.Range (0, levelManager.LevelColors.Length)];
    }

    void CreateGaps () {
        // don't create gaps if it's the first level
        if (levelManager.Levels.Count == 0) {
            return;
        }

        // get random amount of gaps to spawn
        var amount = Random.Range (2, 6);

        var unusedRotations = GetGapRoationList ();

        for (var i = 0; i < amount; i++) {
            var newRotation = GetRandomGapRoation (unusedRotations);
            var go = Instantiate (gapPrefab);
            go.transform.SetParent (gapParent);
            go.transform.localScale = transform.localScale;
            go.name = "Gap " + (i + 1);

            // set random rotation
            var eulerVector = go.transform.eulerAngles;
            eulerVector.z = Math.Abs (newRotation);
            go.transform.localRotation = Quaternion.Euler (eulerVector);

            // bind events
            var gap = go.GetComponentInChildren<LevelGap> ();
            gap.OnPlayerColliding += SetCollider;
        }
    }

    void CreateCollectables () {
        // don't create collectables if it's the first level
        if (levelManager.Levels.Count == 0) {
            return;
        }

        // get random amount of collectables to spawn
        var amount = Random.Range (3, 10);

        for (var i = 0; i < amount; i++) {
            var go = Instantiate (collectablePrefab).GetComponent<Collectable> ();
            go.transform.SetParent (collectableParent);
            go.transform.localScale = transform.localScale;
            go.name = "Collectable " + (i + 1);

            // set random rotation
            var euler = go.transform.eulerAngles;
            euler.z = Random.Range (0f, 360f);
            go.transform.eulerAngles = euler;

            // check overlapping
            var collisions = Physics2D.OverlapCircleAll (go.CollectableTransform.position, go.Collider.radius);
            var loopSafety = 0;
            while (collisions.Length > 1 && loopSafety < 20) {
                euler.z += 10f;
                go.transform.eulerAngles = euler;
                collisions = Physics2D.OverlapCircleAll (go.CollectableTransform.position, go.Collider.radius);
                loopSafety++;
            }
        }
    }

    void CreateEnemies () {
        // don't create enemies if it's the first level
        if (levelManager.Levels.Count <= 10) {
            return;
        }

        // get random amount of enemies to spawn
        var amount = Random.Range (0, levelManager.Levels.Count - 9);

        for (var i = 0; i < amount; i++) {
            var go = Instantiate (enemyPrefab).GetComponent<Enemy> ();
            go.transform.SetParent (enemyParent);
            go.transform.localScale = transform.localScale;
            go.name = "Enemy " + (i + 1);

            // set random rotation
            var euler = go.transform.eulerAngles;
            euler.z = Random.Range (0f, 360f);
            go.transform.eulerAngles = euler;

            // check overlapping
            var collisions = Physics2D.OverlapCircleAll (go.EnemyTransform.position, go.Collider.radius);
            var loopSafety = 0;
            while (collisions.Length > 1 && loopSafety < 20) {
                euler.z += 10f;
                go.gameObject.SetActive (false);
                go.transform.eulerAngles = euler;
                collisions = Physics2D.OverlapCircleAll (go.EnemyTransform.position, go.Collider.radius);
                loopSafety++;
            }
        }
    }

    List<int> GetGapRoationList () {
        var rotations = new List<int> (8);
        for (var i = 0; i < 8; i++) {
            rotations.Add (i);
        }
        return rotations;
    }

    int GetRandomGapRoation (List<int> rotations) {
        var multiple = rotations[Random.Range (0, rotations.Count)];
        var angle = 45;
        var rotation = multiple * angle;
        rotations.Remove (multiple);
        return rotation;
    }

    public void SetCollider (bool state) {
        collider.enabled = state;
    }

    #endregion
}