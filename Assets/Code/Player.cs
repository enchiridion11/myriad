using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        CEILING_CLOCKWISE, // 0
        CEILING_ANTI, // 1
        FLOOR_CLOCKWISE, // 2
        FLOOR_ANTI, // 3
        FALLING // 4
    }

    static Player instance;

    #endregion

    #region Properties

    public static Player Instance {
        get { return instance; }
    }

    #endregion

    #region Events

    public Action<bool> OnPlayerGrounded;

    public Action OnFallingDownGap;

    public Action OnFallingUpGap;

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
                    moveSpeed = 0;
                    animator.Play("player_gapCeilingToFloorClockwise");
                    LevelManager.Instance.SetGravity(LevelManager.GravityDirection.DOWN);
                    break;
                case playerState.FLOOR_ANTI:
                    moveSpeed = 0;
                    animator.Play("player_gapFloorToCeilingAntiClockwise");
                    LevelManager.Instance.SetGravity(LevelManager.GravityDirection.UP);
                    break;
                case playerState.FLOOR_CLOCKWISE:
                    moveSpeed = 0;
                    animator.Play("player_gapFloorToCeilingClockwise");
                    LevelManager.Instance.SetGravity(LevelManager.GravityDirection.UP);
                    break;
                case playerState.CEILING_ANTI:
                    moveSpeed = 0;
                    animator.Play("player_gapCeilingToFloorAntiClockwise");
                    LevelManager.Instance.SetGravity(LevelManager.GravityDirection.DOWN);
                    break;
                case playerState.FALLING:
                    moveSpeed = 0;
                    if (LevelManager.Instance.Gravity == LevelManager.GravityDirection.UP) {
                        if (OnFallingUpGap != null) {
                            OnFallingUpGap();
                        }
                    }
                    else if (LevelManager.Instance.Gravity == LevelManager.GravityDirection.DOWN) {
                        if (OnFallingDownGap != null) {
                            OnFallingDownGap();
                        }
                    }
                    break;
            }

            if (OnPlayerGrounded != null) {
                OnPlayerGrounded(false);
            }
        }

        if (enter.CompareTag("Enemy")) {
            Die();
        }
    }

    #endregion

    void MovePlayer () {
        var euler = playerPivot.eulerAngles;
        euler.z += Time.deltaTime * moveSpeed;
        playerPivot.eulerAngles = euler;
    }

    public void SetMoveSpeed (int direction) {
        moveSpeed = direction;
    }

    public void CeilingToFloor () {
        switch (state) {
            case playerState.CEILING_CLOCKWISE:
                SetPlayerState(4);
                animator.Play("player_ceilingToFloorClockwise");
                break;
            case playerState.CEILING_ANTI:
                SetPlayerState(4);
                animator.Play("player_ceilingToFloorAntiClockwise");
                break;
        }
    }

    public void FloorToCeiling () {
        switch (state) {
            case playerState.FLOOR_CLOCKWISE:
                SetPlayerState(4);
                animator.Play("player_floorToCeilingClockwise");
                break;
            case playerState.FLOOR_ANTI:
                SetPlayerState(4);
                animator.Play("player_floorToCeilingAntiClockwise");
                break;
        }
    }

    public void SetPlayerState (int index) {
        state = (playerState) index;
    }

    public void PlayerGrounded () {
        if (OnPlayerGrounded != null) {
            OnPlayerGrounded(true);
        }
    }

    public void Die () {
        SceneManager.LoadScene("Game");
    }

    #endregion
}