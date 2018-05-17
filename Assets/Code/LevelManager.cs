using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [SerializeField]
    int currentLevel = 1;

    [SerializeField]
    GravityDirection gravity;

    [Header ("Game Objects"), SerializeField]
    Transform levelPool;

    [SerializeField]
    GameObject levelPrefab;

    [Header ("Other"), SerializeField]
    Color[] levelColors;

    [Header ("UI"), SerializeField]
    Text levelText;

    [SerializeField]
    Text bestText;

    [SerializeField]
    Text coinsText;

    [SerializeField]
    Button gravityButton;

    [SerializeField]
    Button directionButton;

    [SerializeField]
    GameObject controlsOverlay;

    Transform levelToHide;

    int coins;

    bool canUseKeyboard = true;

    public enum GravityDirection {
        UP,
        DOWN
    }

    List<Transform> levels;

    static LevelManager instance;

    #endregion

    #region Properties

    public static LevelManager Instance {
        get { return instance; }
    }

    public List<Transform> Levels {
        get { return levels; }
    }

    public Color[] LevelColors {
        get { return levelColors; }
    }

    public Transform LevelPool {
        get { return levelPool; }
    }

    public int CurrentLevel {
        get { return currentLevel; }
    }

    public GravityDirection Gravity {
        get { return gravity; }
    }

    #endregion

    #region Events

    #endregion

    #region Methods

    #region Unity

    void Start () {
        Player.Instance.OnPlayerGrounded += SetGravityButton;
        Player.Instance.OnFallingDownGap += ScaleLevelsUp;
        Player.Instance.OnFallingUpGap += ScaleLevelsDown;
    }

    void Awake () {
        instance = this;
        Initialize ();
    }


    void Update () {
        if (Input.GetKeyDown (KeyCode.LeftControl)) {
            if (controlsOverlay.activeInHierarchy) {
                HideControlsOverlay ();
                return;
            }

            if (canUseKeyboard) {
                ChangeDirection ();
            }
        }

        if (Input.GetKeyDown (KeyCode.Space)) {
            if (controlsOverlay.activeInHierarchy) {
                HideControlsOverlay ();
                return;
            }

            if (canUseKeyboard) {
                SwitchGravity ();
            }
        }
    }

    void OnDisable () {
        Player.Instance.OnPlayerGrounded -= SetGravityButton;
        Player.Instance.OnFallingDownGap -= ScaleLevelsUp;
        Player.Instance.OnFallingUpGap -= ScaleLevelsDown;
    }

    #endregion

    void Initialize () {
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

        levelText.text = currentLevel.ToString ();
        bestText.text = PlayerPrefs.GetInt ("HighScore").ToString ();
    }

    public void ScaleLevelsDown () {
        // prevent player from going beyond lowest level
        if (offset >= 2) {
            return;
        }

        // if offset is positive or zero
        if (offset >= 0) {
            SetLevelColliders (false);
            //StopAllCoroutines();

            // scale levels down
            for (var i = 0; i <= maxLevels - Math.Abs (offset) - 1; i++) {
                var newScale = (float) Math.Pow (scaleRatio, i + offset);
                // levels[i].localScale = new Vector2(newScale, newScale);
                // StartCoroutine (AnimateScaleDown (levels[i], levels[i].localScale.x, newScale));
                AnimateScaleDown (levels[i], new Vector3 (newScale, newScale, newScale));
            }

            if (maxLevels - Math.Abs (offset) - 1 >= 0) {
                levels[maxLevels - Math.Abs (offset) - 1].GetComponent<Level> ().HideLevel ();
            }
        }

        // if offset is negative
        else if (offset < 0) {
            SetLevelColliders (false);
            // StopAllCoroutines();

            // scale levels down
            for (var i = Math.Abs (offset) - 1; i <= maxLevels + Math.Abs (offset) - 1; i++) {
                var newScale = (float) Math.Pow (scaleRatio, i + offset);
                // levels[i].localScale = new Vector2(newScale, newScale);
                //StartCoroutine (AnimateScaleDown (levels[i], levels[i].localScale.x, newScale));
                AnimateScaleDown (levels[i], new Vector3 (newScale, newScale, newScale));
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

            SetLevelColliders (true);
            // StopAllCoroutines();

            // scale levels up
            for (var i = Math.Abs (offset); i <= maxLevels + Math.Abs (offset); i++) {
                var newScale = (float) Math.Pow (scaleRatio, i - 2 + offset);
                // levels[i].localScale = new Vector2(newScale, newScale);
                // StartCoroutine (AnimateScaleUp (levels[i], levels[i].localScale.x, newScale));
                AnimateScaleUp (levels[i], new Vector3 (newScale, newScale, newScale));
            }

            // if a level at the end goes out of bounds, add to the pool
            // if (Math.Abs(offset) <= levels.Count - 1) {
            //  levelToHide = levels[Math.Abs(offset)];
            // print (levelToHide.name);
            //  }
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

            SetLevelColliders (true);
            // StopAllCoroutines();

            // scale levels up
            for (var i = 0; i <= maxLevels - Math.Abs (offset); i++) {
                var newScale = (float) Math.Pow (scaleRatio, i - 2 + offset);
                // levels[i].localScale = new Vector2(newScale, newScale);
                // StartCoroutine (AnimateScaleUp (levels[i], levels[i].localScale.x, newScale));
                AnimateScaleUp (levels[i], new Vector3 (newScale, newScale, newScale));
            }

            // if a level at the start goes out of bounds, add to the pool
            if (Math.Abs (offset) <= 0) {
                levelToHide = levels[Math.Abs (offset)];
                // print (levelToHide.name);
            }
        }

        offset--;
        currentLevel++;
        levelText.text = currentLevel.ToString ();
    }

    /* public IEnumerator AnimateScaleDown (Transform level, float startValue, float endValue) {
         while (startValue > endValue) {
             startValue -= Time.deltaTime * scaleSpeed * startValue;
             level.localScale = new Vector2 (startValue, startValue);
             yield return null;
         }
 
         // set it to the end value in case precision was lost
         level.localScale = new Vector2 (endValue, endValue);
     }*/

    /*public IEnumerator AnimateScaleUp (Transform level, float startValue, float endValue) {
        while (startValue < endValue) {
            startValue += Time.deltaTime * scaleSpeed * startValue;
            level.localScale = new Vector2 (startValue, startValue);
            yield return null;
        }

        // set it to the end value in case precision was lost
        level.localScale = new Vector2 (endValue, endValue);

        // only hide the level once it's done animating
        if (offset < 0) {
            if (Math.Abs (offset) - 1 < levels.Count - maxLevels) {
           //     print (levels.Count);
             //   print (maxLevels);
           //     print (levels[Math.Abs (offset) - 1].name);
            }
            levels[Math.Abs (offset) - 1].GetComponent<Level> ().HideLevel ();
        }
    }*/

    void AnimateScaleUp (Transform level, Vector3 scale) {
        iTween.ScaleTo (level.gameObject,
            iTween.Hash ("scale", scale, "time", scaleSpeed, "easeType", "linear", "oncomplete", "OnLevelScaledUp", "oncompletetarget", gameObject, "oncompleteparams", level));
    }

    void AnimateScaleDown (Transform level, Vector3 scale) {
        iTween.ScaleTo (level.gameObject, iTween.Hash ("scale", scale, "time", scaleSpeed, "easeType", "linear"));
    }

    void OnLevelScaledUp (Transform levelTransform) {
        if (levelTransform.localScale.x > 2) {
            levelTransform.GetComponent<Level> ().HideLevel ();
        }
    }

    void CreateNewLevel () {
        var go = Instantiate (levelPrefab).GetComponent<Level> ();
        go.transform.SetParent (transform);
        go.Initialize ();
        go.name = "Level " + levels.Count;
        go.LevelNumber = levels.Count + 1;

        var newScale = (float) Math.Pow (scaleRatio, maxLevels - 1);
        go.transform.localScale = new Vector2 (newScale, newScale);

        levels.Add (go.transform);
    }

    void SetLevelColliders (bool isScalingUp) {
        if (isScalingUp) {
            // enable next level collider
            levels[currentLevel].GetComponent<Level> ().SetCollider (true);

            // disable current level collider
            levels[currentLevel - 1].GetComponent<Level> ().SetCollider (false);
        }
        else {
            // enable next level collider
            levels[currentLevel - 2].GetComponent<Level> ().SetCollider (true);

            // disable current level collider
            levels[currentLevel - 1].GetComponent<Level> ().SetCollider (false);
        }
    }

    public void SwitchGravity () {
        if (gravity == GravityDirection.UP) {
            ScaleLevelsUp ();
            gravity = GravityDirection.DOWN;
            Player.Instance.CeilingToFloor ();
        }
        else if (gravity == GravityDirection.DOWN) {
            ScaleLevelsDown ();
            gravity = GravityDirection.UP;
            Player.Instance.FloorToCeiling ();
        }
    }

    public void SetGravity (GravityDirection state) {
        gravity = state;
    }

    void SetGravityButton (bool isGrounded) {
        gravityButton.interactable = isGrounded;
        directionButton.interactable = isGrounded;
        canUseKeyboard = isGrounded;
    }

    public void ChangeDirection () {
        Player.Instance.ChangeDirection ();
    }

    public void AddCollectable () {
        coins++;
        coinsText.text = coins.ToString ();
    }

    public void HideControlsOverlay () {
        controlsOverlay.SetActive (false);
    }

    public void RestartGame () {
        var highscore = PlayerPrefs.GetInt ("HighScore");
        if (currentLevel > highscore) {
            PlayerPrefs.SetInt ("HighScore", currentLevel);
        }

        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }

    #endregion
}