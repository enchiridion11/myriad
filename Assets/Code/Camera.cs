using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour {
    #region Fields

    [SerializeField]
    int rotateSpeed;

    #endregion

    #region Properties

    #endregion

    #region Methods

    #region Unity

    void Start () {
        Player.Instance.OnPlayerDirectionChange += ChangeRotationDirection;
    }

    void Update () {
        RotateCamera();
    }

    void OnDisable () {
        Player.Instance.OnPlayerDirectionChange -= ChangeRotationDirection;
    }

    #endregion

    void RotateCamera () {
        var euler = transform.eulerAngles;
        euler.z += Time.deltaTime * rotateSpeed;
        transform.eulerAngles = euler;
    }

    void ChangeRotationDirection () {
        rotateSpeed *= -1;
    }

    #endregion
}