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
    bool isGrounded = true;

    [SerializeField]
    PlayerState state;

    int previousMoveSpeed;

    enum PlayerState {
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
    
    public bool IsGrounded {
        get { return isGrounded; }
    }

    #endregion

    #region Events

    public Action<bool> OnPlayerGrounded;

    public Action OnFallingDownGap;

    public Action OnFallingUpGap;

    public Action OnPlayerDirectionChange;

    #endregion

    #region Methods

    #region Unity

    void Awake () {
        instance = this;
        previousMoveSpeed = moveSpeed;
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.S)) {
            CeilingToFloor ();
        }

        if (Input.GetKeyDown (KeyCode.W)) {
            FloorToCeiling ();
        }

        MovePlayer ();
    }

    void OnTriggerEnter2D (Collider2D other) {
        if (other.CompareTag ("Gap")) {
            if (isGrounded) {
                switch (state) {
                    case PlayerState.CEILING_CLOCKWISE:
                        // print("CEILING_CLOCKWISE");
                        moveSpeed = 0;
                        animator.Play ("player_gapCeilingToFloorClockwise");
                        LevelManager.Instance.SetGravity (LevelManager.GravityDirection.DOWN);
                        if (OnPlayerDirectionChange != null) {
                            OnPlayerDirectionChange ();
                        }
                        break;
                    case PlayerState.FLOOR_ANTI:
                        //  print("FLOOR_ANTI");
                        moveSpeed = 0;
                        animator.Play ("player_gapFloorToCeilingAntiClockwise");
                        LevelManager.Instance.SetGravity (LevelManager.GravityDirection.UP);
                        if (OnPlayerDirectionChange != null) {
                            OnPlayerDirectionChange ();
                        }
                        break;
                    case PlayerState.FLOOR_CLOCKWISE:
                        // print("FLOOR_CLOCKWISE");
                        moveSpeed = 0;
                        animator.Play ("player_gapFloorToCeilingClockwise");
                        LevelManager.Instance.SetGravity (LevelManager.GravityDirection.UP);
                        if (OnPlayerDirectionChange != null) {
                            OnPlayerDirectionChange ();
                        }
                        break;
                    case PlayerState.CEILING_ANTI:
                        // print("CEILING_ANTI");
                        moveSpeed = 0;
                        animator.Play ("player_gapCeilingToFloorAntiClockwise");
                        LevelManager.Instance.SetGravity (LevelManager.GravityDirection.DOWN);
                        if (OnPlayerDirectionChange != null) {
                            OnPlayerDirectionChange ();
                        }
                        break;
                }
            }
            else {
                // print("FALLING");
                moveSpeed = 0;
                if (LevelManager.Instance.Gravity == LevelManager.GravityDirection.UP) {
                    if (OnFallingUpGap != null) {
                        OnFallingUpGap ();
                    }
                }
                else if (LevelManager.Instance.Gravity == LevelManager.GravityDirection.DOWN) {
                    if (OnFallingDownGap != null) {
                        OnFallingDownGap ();
                    }
                }
            }

            if (OnPlayerGrounded != null) {
                OnPlayerGrounded (false);
            }

            return;
        }

        if (other.CompareTag ("Level")) {
            isGrounded = true;
            switch (state) {
                case PlayerState.CEILING_CLOCKWISE:
                    // print("CEILING_CLOCKWISE");
                    animator.Play ("player_moveFloorClockwise");
                    break;
                case PlayerState.FLOOR_ANTI:
                    //  print("FLOOR_ANTI");
                    animator.Play ("player_moveCeilingAntiClockwise");
                    break;
                case PlayerState.FLOOR_CLOCKWISE:
                    // print("FLOOR_CLOCKWISE");
                    animator.Play ("player_moveCeilingClockwise");
                    break;
                case PlayerState.CEILING_ANTI:
                    // print("CEILING_ANTI");
                    animator.Play ("player_moveFloorAntiClockwise");
                    break;
            }
        }

        if (other.CompareTag ("Collectable")) {
            Destroy (other.transform.parent.gameObject);
            Collect ();
        }

        if (other.CompareTag ("Enemy")) {
            Die ();
        }
    }

    #endregion

    void MovePlayer () {
        var euler = playerPivot.eulerAngles;
        euler.z += Time.deltaTime * moveSpeed;
        playerPivot.eulerAngles = euler;
    }

    public void SetMoveSpeed (int direction) {
        moveSpeed = Math.Abs (previousMoveSpeed) * direction;
    }

    public void CeilingToFloor () {
        isGrounded = false;
        moveSpeed = 0;
        
        if (OnPlayerGrounded != null) {
            OnPlayerGrounded (isGrounded);
        }

        switch (state) {
            case PlayerState.CEILING_CLOCKWISE:
                animator.Play ("player_ceilingToFloorClockwise");
                break;
            case PlayerState.CEILING_ANTI:
                animator.Play ("player_ceilingToFloorAntiClockwise");
                break;
        }
    }

    public void FloorToCeiling () {
        isGrounded = false;
        moveSpeed = 0;
        
        if (OnPlayerGrounded != null) {
            OnPlayerGrounded (isGrounded);
        }

        switch (state) {
            case PlayerState.FLOOR_CLOCKWISE:
                animator.Play ("player_floorToCeilingClockwise");
                break;
            case PlayerState.FLOOR_ANTI:
                animator.Play ("player_floorToCeilingAntiClockwise");
                break;
        }
    }

    public void SetPlayerState (int index) {
        state = (PlayerState) index;
    }

    public void PlayerGrounded () {
        if (OnPlayerGrounded != null) {
            OnPlayerGrounded (true);
        }
    }

    public void ChangeDirection () {
        switch (state) {
            case PlayerState.CEILING_CLOCKWISE:
                SetPlayerState (1);
                animator.Play ("player_moveCeilingAntiClockwise");
                break;
            case PlayerState.CEILING_ANTI:
                SetPlayerState (0);
                animator.Play ("player_moveCeilingClockwise");
                break;
            case PlayerState.FLOOR_CLOCKWISE:
                SetPlayerState (3);
                animator.Play ("player_moveFloorAntiClockwise");
                break;
            case PlayerState.FLOOR_ANTI:
                SetPlayerState (2);
                animator.Play ("player_moveFloorClockwise");
                break;
        }

        if (OnPlayerDirectionChange != null) {
            OnPlayerDirectionChange ();
        }
    }

    public void Die () {
        LevelManager.Instance.RestartGame ();
    }

    void Collect () {
        LevelManager.Instance.AddCollectable ();
    }

    #endregion
}