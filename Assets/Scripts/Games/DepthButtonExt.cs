using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/** Pairs with DepthButtonSer; add both together. */
[RequireComponent(typeof(DepthButtonSer))]
public class DepthButtonExt : Button {
    // Components
    private DepthButtonSer depthButtonSer;
    // Properties
    private bool isPointerDown = false;


    // ----------------------------------------------------------------
    //  Awake
    // ----------------------------------------------------------------
    override protected void Awake() {
        base.Awake();
        
        depthButtonSer = GetComponent<DepthButtonSer>();
        if (depthButtonSer == null) {
            Debug.LogError("Whoa! No DepthButtonSer attached to DepthButtonExt. Gotta pair 'em together, ma'am.");
        }
    }

    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public override void OnPointerEnter(PointerEventData eventData) {
        base.OnPointerEnter(eventData);
        if (isPointerDown) {
            depthButtonSer.OnPointerDownEnter();
        }
    }
    public override void OnPointerExit(PointerEventData eventData) {
        base.OnPointerExit(eventData);
        if (isPointerDown) {
            depthButtonSer.OnPointerDownExit();
        }
    }
    public override void OnPointerDown(PointerEventData eventData) {
        base.OnPointerDown(eventData);
        depthButtonSer.OnPointerDown();
        isPointerDown = true;
    }
    public override void OnPointerUp(PointerEventData eventData) {
        base.OnPointerUp(eventData);
        depthButtonSer.OnPointerUp();
        isPointerDown = false;
    }

}
