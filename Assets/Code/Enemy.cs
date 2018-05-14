using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    #region Fields

    [SerializeField]
    Transform enemyTransform;

    [SerializeField]
    CircleCollider2D collider;

    #endregion

    #region Properties

    public CircleCollider2D Collider {
        get { return collider; }
    }

    public Transform EnemyTransform {
        get { return enemyTransform; }
    }

    #endregion

    #region Methods

    #region Unity

    #endregion

    #endregion
}