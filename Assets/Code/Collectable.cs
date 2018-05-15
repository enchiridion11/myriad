using UnityEngine;

public class Collectable : MonoBehaviour {
    #region Fields

    [SerializeField]
    Transform collectableTransform;

    [SerializeField]
    CircleCollider2D collider;

    #endregion

    #region Properties

    public CircleCollider2D Collider {
        get { return collider; }
    }

    public Transform CollectableTransform {
        get { return collectableTransform; }
    }

    #endregion

    #region Methods

    #region Unity

    #endregion

    #endregion
}