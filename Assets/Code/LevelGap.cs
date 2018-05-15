using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGap : MonoBehaviour {
    #region Fields

    #endregion

    #region Properties

    #endregion

    #region Events

    public Action<bool> OnPlayerColliding;

    #endregion

    #region Methods

    #region Unity

    void OnTriggerEnter2D (Collider2D other) {
        if (other.CompareTag ("Player")) {
            if (!Player.Instance.IsGrounded) {
                if (OnPlayerColliding != null) {
                    OnPlayerColliding (false);
                }
            }
        }
    }

    void OnTriggerExit2D (Collider2D other) {
        if (other.CompareTag ("Player")) {
            if (!Player.Instance.IsGrounded) {
                if (OnPlayerColliding != null) {
                    OnPlayerColliding (true);
                }
            }
        }
    }

    #endregion

    #endregion
}