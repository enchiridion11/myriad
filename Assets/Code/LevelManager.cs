using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {
    #region Fields 

    [Header ("Settings"), SerializeField]
    int totalLevels;

    [SerializeField]
    int maxLevels;

    [SerializeField]
    int offset;

    [SerializeField]
    float scaleRatio;

    [SerializeField]
    float scaleSpeed;

    [Header ("Game Objects"), SerializeField]
    Transform levelPool;

    [SerializeField]
    GameObject levelPrefab;

    [Header ("Other"), SerializeField]
    Color[] levelColors;

    [SerializeField]
    Text offsetText;

    [SerializeField]
    Text levelText;

    int currentLevel = 1;

    public enum gravityDirection {
        UP,
        DOWN
    }

    gravityDirection gravity;

    List<Transform> levels;

    static LevelManager instance;

    #endregion

    #region Properties

    public static LevelManager Instance {
        get { return instance; }
    }

    public Color[] LevelColors {
        get { return levelColors; }
    }

    public Transform LevelPool {
        get { return levelPool; }
    }

    #endregion

    #region Events

    public Action OnLevelsScaled;

    #endregion

    #region Methods

    #region Unity

    void Awake () {
        instance = this;
        Initialize ();
    }


    void Update () {
        if (Input.GetKeyDown (KeyCode.UpArrow)) {
            ScaleLevelsUp ();
        }

        if (Input.GetKeyDown (KeyCode.DownArrow)) {
            ScaleLevelsDown ();
        }
    }

    #endregion

    void Initialize () {
        gravity = gravityDirection.UP;

        levels = new List<Transform> (totalLevels);

        for (var i = 0; i < totalLevels; i++) {
            var go = Instantiate (levelPrefab).GetComponent<Level> ();
            go.transform.SetParent (transform);
            go.Initialize ();

            var newScale = (float) Math.Pow (scaleRatio, i - 1 + offset);
            go.transform.localScale = new Vector2 (newScale, newScale);


            go.name = "Level " + i;
            levels.Add (go.transform);
        }

        offsetText.text = offset.ToString ();
        levelText.text = currentLevel.ToString ();
    }

    public void ScaleLevelsDown () {
        // prevent player from going beyond lowest level
        if (offset >= 2) {
            return;
        }

        // if offset is positive or zero
        if (offset >= 0) {
            // move levels to the right
            for (var i = 0; i <= maxLevels - Math.Abs (offset) - 1; i++) {
                var newScale = (float) Math.Pow (scaleRatio, i + offset);
                // levels[i].localScale = new Vector2(newScale, newScale);
                StartCoroutine (AnimateScaleDown (levels[i], levels[i].localScale.x, newScale));
            }

            if (maxLevels - Math.Abs (offset) - 1 >= 0) {
                levels[maxLevels - Math.Abs (offset) - 1].GetComponent<Level> ().HideLevel ();
            }
        }

        // if offset is negative
        else if (offset < 0) {
            // move levels to the right
            for (var i = Math.Abs (offset) - 1; i <= maxLevels + Math.Abs (offset) - 1; i++) {
                var newScale = (float) Math.Pow (scaleRatio, i + offset);
                // levels[i].localScale = new Vector2(newScale, newScale);
                StartCoroutine (AnimateScaleDown (levels[i], levels[i].localScale.x, newScale));
            }

            // add level at the end to the pool
            levels[maxLevels + Math.Abs (offset) - 1].GetComponent<Level> ().HideLevel ();

            // if there is a previous level, get it from the pool
            if (Math.Abs (offset) - 1 <= levels.Count - 1) {
                levels[Math.Abs (offset) - 1].GetComponent<Level> ().ShowLevel ();
            }
        }

        offset++;
        currentLevel--;
        offsetText.text = offset.ToString ();
        levelText.text = currentLevel.ToString ();
    }

    public void ScaleLevelsUp () {
        // if offset is negative
        if (offset < 0) {
            // if there is another level, get it from pool
            if (maxLevels + Math.Abs (offset) < levels.Count) {
                levels[maxLevels + Math.Abs (offset)].GetComponent<Level> ().ShowLevel ();
            }
            // if there are no more levels, create one
            else {
                CreateNewLevel ();
            }

            // scale levels up
            for (var i = Math.Abs (offset); i <= maxLevels + Math.Abs (offset); i++) {
                var newScale = (float) Math.Pow (scaleRatio, i - 2 + offset);
                // levels[i].localScale = new Vector2(newScale, newScale);
                StartCoroutine (AnimateScaleUp (levels[i], levels[i].localScale.x, newScale));
            }

            // if a level at the end goes out of bounds, add to the pool
            if (Math.Abs (offset) <= levels.Count - 1) {
                levels[Math.Abs (offset)].GetComponent<Level> ().HideLevel ();
            }
        }

        // if offset is positive or zero
        else if (offset >= 0) {
            // if there is another level at the end, get it from pool
            if (maxLevels - Math.Abs (offset) < levels.Count) {
                levels[maxLevels - Math.Abs (offset)].GetComponent<Level> ().ShowLevel ();
            }
            // if there are no more levels at the end, create one
            else {
                CreateNewLevel ();
            }

            // scale levels up
            for (var i = 0; i <= maxLevels - Math.Abs (offset); i++) {
                var newScale = (float) Math.Pow (scaleRatio, i - 2 + offset);
                // levels[i].localScale = new Vector2(newScale, newScale);
                StartCoroutine (AnimateScaleUp (levels[i], levels[i].localScale.x, newScale));
            }

            // if a level at the start goes out of bounds, add to the pool
            if (Math.Abs (offset) <= 0) {
                levels[Math.Abs (offset)].GetComponent<Level> ().HideLevel ();
            }
        }

        offset--;
        currentLevel++;
        offsetText.text = offset.ToString ();
        levelText.text = currentLevel.ToString ();
    }

    public IEnumerator AnimateScaleDown (Transform level, float startValue, float endValue) {
        while (startValue > endValue) {
            startValue -= Time.deltaTime * scaleSpeed; // * startValue;
            level.localScale = new Vector2 (startValue, startValue);
            yield return null;
        }
        level.localScale = new Vector2 (endValue, endValue);

        //TODO: move until after level done scaling
        // Player.Instance.SwitchGracity(gravity);
    }

    public IEnumerator AnimateScaleUp (Transform level, float startValue, float endValue) {
        while (startValue < endValue) {
            startValue += Time.deltaTime * scaleSpeed; // * startValue;
            level.localScale = new Vector2 (startValue, startValue);
            yield return null;
        }
        level.localScale = new Vector2 (endValue, endValue);

        //TODO: move until after level done scaling
        // Player.Instance.SwitchGracity(gravity);
    }

    void CreateNewLevel () {
        var go = Instantiate (levelPrefab).GetComponent<Level> ();
        go.transform.SetParent (transform);
        go.Initialize ();
        go.name = "Level " + levels.Count;

        var newScale = (float) Math.Pow (scaleRatio, maxLevels + 2);
        go.transform.localScale = new Vector2 (newScale, newScale);

        levels.Add (go.transform);
    }

    public void SwitchGravity () {
        if (gravity == gravityDirection.UP) {
            ScaleLevelsUp ();
            gravity = gravityDirection.DOWN;
            Player.Instance.CeilingToFloor ();
        }
        else if (gravity == gravityDirection.DOWN) {
            ScaleLevelsDown ();
            gravity = gravityDirection.UP;
            Player.Instance.FloorToCeiling ();
        }
    }

    #endregion
}