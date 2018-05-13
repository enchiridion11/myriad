using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    #region Fields

    [SerializeField]
    Transform playertTransform;

    [SerializeField]
    int speed;

    [SerializeField]
    int transitionSpeed;

    [SerializeField]
    float onHeight;

    [SerializeField]
    float underHeight;

    static Player instance;

    #endregion

    #region Properties

    public static Player Instance {
        get { return instance; }
    }

    #endregion

    #region Methods

    #region Unity

    void Awake () {
        instance = this;
    }


    void Update () {
        var euler = transform.eulerAngles;
        euler.z += Time.deltaTime * speed;
        transform.eulerAngles = euler;
    }

    #endregion

    public void SwitchGracity (LevelManager.gravityDirection dir) {
        switch (dir) {
            case LevelManager.gravityDirection.DOWN:
                playertTransform.localPosition = new Vector2(0f, onHeight);
                break;
            case LevelManager.gravityDirection.UP:
                playertTransform.localPosition = new Vector2(0f, underHeight);
                break;
        }
    }

    public IEnumerator SwitchToUnderHeight () {
        var currentHeight = playertTransform.localPosition.y;
        while (currentHeight > underHeight) {
            currentHeight -= Time.deltaTime * transitionSpeed;
            playertTransform.localPosition = new Vector2(0f, currentHeight);
            yield return null;
        }
        playertTransform.localPosition = new Vector2(0f, underHeight);
    }

    public IEnumerator SwitchToOnHeight () {
        var currentHeight = playertTransform.localPosition.y;
        while (currentHeight < onHeight) {
            currentHeight += Time.deltaTime * transitionSpeed;
            playertTransform.localPosition = new Vector2(0f, currentHeight);
            yield return null;
        }
        playertTransform.localPosition = new Vector2(0f, onHeight);
    }

    #endregion
}