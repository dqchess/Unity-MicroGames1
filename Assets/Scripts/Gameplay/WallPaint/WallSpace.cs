using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpace : MonoBehaviour {
    // Components
    [SerializeField] private SpriteRenderer bodySprite;


    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize(Transform tf_parent, Vector2 _pos) {
        this.transform.SetParent(tf_parent);
        this.transform.localScale = Vector3.one;
        this.transform.localEulerAngles = Vector3.zero;
        this.transform.localPosition = _pos;
    }



}
