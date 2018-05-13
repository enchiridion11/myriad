using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManagerTest : MonoBehaviour {
    [Header("Settings"), SerializeField]
    int totalLevels;

    [SerializeField]
    int maxLevels;

    [SerializeField]
    int offset;

    [SerializeField]
    int spacing;

    [Header("UI"), SerializeField]
    Text offsetText;

    [SerializeField]
    Text totalLevelsText;

    [Header("Game Objects"), SerializeField]
    Transform levelPool;

    [SerializeField]
    GameObject levelPrefab;

    [SerializeField]
    GameObject levelEnd;

    List<Transform> levels;

    static LevelManagerTest instance;

    public static LevelManagerTest Instance {
        get { return instance; }
    }

    public Transform LevelPool {
        get { return levelPool; }
        set { levelPool = value; }
    }

    void Awake () {
        instance = this;
    }

    void Start () {
        levels = new List<Transform>(totalLevels);
        for (var i = 0; i < totalLevels; i++) {
            var go = Instantiate(levelPrefab);
            go.transform.SetParent(transform);
            go.GetComponent<LevelTest>().Set();

            if (i + offset < 0) {
                go.transform.position = new Vector3(-spacing, 0, 0);
                go.GetComponent<LevelTest>().Hide();
            }
            else if (i + offset < maxLevels) {
                go.transform.position = new Vector3(i * spacing + offset * spacing, 0, 0);
            }
            else {
                go.transform.position = new Vector3(maxLevels * spacing, 0, 0);
                go.GetComponent<LevelTest>().Hide();
            }

            go.GetComponent<LevelTest>().Index = i.ToString();
            go.name = "Level " + i;
            levels.Add(go.transform);
            levelEnd.transform.position = new Vector3(maxLevels * spacing - 1, 0, 0);
        }
        offsetText.text = offset.ToString();
        totalLevelsText.text = levels.Count.ToString();
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            MoveLeft();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            MoveRight();
        }
    }

    void CreateNewLevel () {
        var go = Instantiate(levelPrefab);
        go.transform.SetParent(transform);
        go.transform.position = new Vector3(maxLevels * spacing, 0, 0);
        go.GetComponent<LevelTest>().Set();
        go.GetComponent<LevelTest>().Index = levels.Count.ToString();
        go.name = "Level " + levels.Count;
        levels.Add(go.transform);
        totalLevelsText.text = levels.Count.ToString();
    }

    void MoveLeft () {
        // if offset is negative
        if (offset < 0) {
            // if there is another level, get it from pool
            if (maxLevels + Math.Abs(offset) < levels.Count) {
                levels[maxLevels + Math.Abs(offset)].GetComponent<LevelTest>().Show();
            }
            // if there are no more levels, create one
            else {
                CreateNewLevel();
            }

            // move levels to the left
            for (var i = Math.Abs(offset); i <= maxLevels + Math.Abs(offset); i++) {
                levels[i].position = new Vector3(levels[i].position.x - spacing, 0, 0);
            }

            // if a level at the end goes out of bounds, add to the pool
            if (Math.Abs(offset) <= levels.Count - 1) {
                levels[Math.Abs(offset)].GetComponent<LevelTest>().Hide();
            }
        }

        // if offset is positive or zero
        else if (offset >= 0) {
            // if there is another level at the end, get it from pool
            if (maxLevels - Math.Abs(offset) < levels.Count) {
                levels[maxLevels - Math.Abs(offset)].GetComponent<LevelTest>().Show();
            }
            // if there are no more levels at the end, create one
            else {
                CreateNewLevel();
            }

            // move levels to the left
            for (var i = 0; i <= maxLevels - Math.Abs(offset); i++) {
                levels[i].position = new Vector3(levels[i].position.x - spacing, 0, 0);
            }

            // if a level at the start goes out of bounds, add to the pool
            if (Math.Abs(offset) <= 0) {
                levels[Math.Abs(offset)].GetComponent<LevelTest>().Hide();
            }
        }

        offset--;
        offsetText.text = offset.ToString();
    }

    void MoveRight () {
        
        // prevent player from going beyond lowest level
        if (offset >= 2) {
            return;
        }
        
        // if offset is positive or zero
        if (offset >= 0) {
            // move levels to the right
            for (var i = 0; i <= maxLevels - Math.Abs(offset) - 1; i++) {
                levels[i].position = new Vector3(levels[i].position.x + spacing, 0, 0);
            }
            
            if (maxLevels - Math.Abs(offset) - 1 >= 0) {
                levels[maxLevels - Math.Abs(offset) - 1].GetComponent<LevelTest>().Hide();
            }
        }

        // if offset is negative
        else if (offset < 0) {
            // move levels to the right
            for (var i = Math.Abs(offset) - 1; i <= maxLevels + Math.Abs(offset) - 1; i++) {
                levels[i].position = new Vector3(levels[i].position.x + spacing, 0, 0);
            }

            // add level at the end to the pool
            levels[maxLevels + Math.Abs(offset) - 1].GetComponent<LevelTest>().Hide();

            // if there is a previous level, get it from the pool
            if (Math.Abs(offset) - 1 <= levels.Count - 1) {
                levels[Math.Abs(offset) - 1].GetComponent<LevelTest>().Show();
            }
        }

        offset++;
        offsetText.text = offset.ToString();
    }
}