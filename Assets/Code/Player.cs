using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    #region Fields

    [SerializeField]
    Transform playertTransform;

    [SerializeField]
    Animator animator;

    [SerializeField]
    int moveSpeed;

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
        if (Input.GetKeyDown(KeyCode.S)) {
            CeilingToFloor();
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            FloorToCeiling();
        }
        MovePlayer ();
    }

    #endregion

    public IEnumerator SwitchToUnderHeight () {
        var currentHeight = playertTransform.localPosition.y;
        while (currentHeight > underHeight) {
            currentHeight -= Time.deltaTime * transitionSpeed;
            playertTransform.localPosition = new Vector2 (0f, currentHeight);
            yield return null;
        }
        playertTransform.localPosition = new Vector2 (0f, underHeight);
    }

    public IEnumerator SwitchToOnHeight () {
        var currentHeight = playertTransform.localPosition.y;
        while (currentHeight < onHeight) {
            currentHeight += Time.deltaTime * transitionSpeed;
            playertTransform.localPosition = new Vector2 (0f, currentHeight);
            yield return null;
        }
        playertTransform.localPosition = new Vector2 (0f, onHeight);
    }

    void MovePlayer () {
        var euler = transform.eulerAngles;
        euler.z += Time.deltaTime * moveSpeed;
        transform.eulerAngles = euler;
    }

    void SetMoveSpeed () {
        animator.SetFloat ("MoveSpeed", moveSpeed);
    }

    public void CeilingToFloor () {
        animator.Play ("player_ceilingToFloor");
    }
    
    public void FloorToCeiling () {
        animator.Play ("player_floorToCeiling");
    }

    #endregion
}