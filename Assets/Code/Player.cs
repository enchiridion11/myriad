using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    #region Fields

    [SerializeField]
    Transform playerPivot;

    [SerializeField]
    Animator animator;

    [SerializeField]
    int moveSpeed;

    [SerializeField]
    playerState state;

    enum playerState {
        CEILING_CLOCKWISE,
        CEILING_ANTI,
        FLOOR_CLOCKWISE,
        FLOOR_ANTI,
        TRANSITION
    }


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
        MovePlayer();
    }

    void OnTriggerEnter2D (Collider2D enter) {
        if (enter.CompareTag("Gap")) {
            switch (state) {
                case playerState.CEILING_CLOCKWISE:
                    print("CEILING_CLOCKWISE");
                    moveSpeed = 0;
                    animator.Play("player_gapCeilingClockwise");
                    state = playerState.FLOOR_ANTI;
                    break;
            }
        }
    }

    #endregion

    void MovePlayer () {
        var euler = playerPivot.eulerAngles;
        euler.z += Time.deltaTime * moveSpeed;
        playerPivot.eulerAngles = euler;
    }

    public void SetMoveSpeed (int speed) {
        moveSpeed = speed;
    }

    public void CeilingToFloor () {
        animator.Play("player_ceilingToFloor");
    }

    public void FloorToCeiling () {
        animator.Play("player_floorToCeiling");
    }

    #endregion
}