using UnityEngine;
using System.Collections;

public class ScaleSample : MonoBehaviour {
    void Start () {
        //	iTween.RotateBy(gameObject, iTween.Hash("x", .25, "easeType", "easeInOutBack", "loopType", "pingPong", "delay", .4));
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.Space)) {
            AnimateScaleUp (new Vector3 (2, 2, 2));
        }
    }


    public void AnimateScaleUp (Vector3 scale) {
        iTween.ScaleTo (gameObject, iTween.Hash ("scale", scale, "time", 1f, "easeType", "easeInOutQuad", "oncomplete", "Callback"));
    }

    void Callback () {
        print ("worked!");
    }
}