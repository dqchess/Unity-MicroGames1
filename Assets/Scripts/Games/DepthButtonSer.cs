using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/** Pairs with DepthButtonExt; add both together. */
public class DepthButtonSer : MonoBehaviour {
    // Constants
    private static float depthEasing = 0.36f; // higher is faster.
    // Components
    [SerializeField] private RectTransform rt_bottom=null;
    [SerializeField] private RectTransform rt_top=null;
    private Button myButton;
    // Properties
    [SerializeField] private float depthNeutral = 5f;
    [SerializeField] private float depthPressed = 0f;
    private float depth; // how "tall" the button is. Dist between bottom and top images.
    // Coroutines
    private Coroutine c_animateDepth;


    // Setters (Private)
    private float Depth {
        get { return depth; }
        set {
            depth = value;
            float topPosY = depth - depthNeutral;
            rt_top.anchoredPosition = new Vector2(rt_top.anchoredPosition.x, topPosY);
        }
    }


    // ----------------------------------------------------------------
    //  Awake / Start
    // ----------------------------------------------------------------
    private void Awake() {
        myButton = GetComponent<Button>();
        if (myButton == null) { Debug.LogError("Whoa! DepthButtonSer needs a DepthButtonExt component, too."); }
    }
    private void Start() {
        // Start un-pressed.
        Depth = depthNeutral;
        rt_bottom.anchoredPosition = new Vector2(rt_bottom.anchoredPosition.x, -depthNeutral);
    }


    // ----------------------------------------------------------------
    //  Events (Called by my DepthButtonExt)
    // ----------------------------------------------------------------
    public void OnPointerDownEnter() {
        if (!myButton.IsInteractable()) { return; } // Not interactable? No animating.
        AnimateDepth(depthPressed);
    }
    public void OnPointerDownExit() {
        if (!myButton.IsInteractable()) { return; } // Not interactable? No animating.
        AnimateDepth(depthNeutral);
    }
    public void OnPointerDown() {
        if (!myButton.IsInteractable()) { return; } // Not interactable? No animating.
        AnimateDepth(depthPressed);
    }
    public void OnPointerUp() {
        if (!myButton.IsInteractable()) { return; } // Not interactable? No animating.
        AnimateDepth(depthNeutral);
    }



    // ----------------------------------------------------------------
    //  Animating
    // ----------------------------------------------------------------
    private void AnimateDepth(float targetDepth) {
        if (!gameObject.activeInHierarchy) { return; } // Safety check: If I'm not active in the hierarchy, don't animate.
        
        if (c_animateDepth != null) { StopCoroutine(c_animateDepth); }
        c_animateDepth = StartCoroutine(Coroutine_AnimateDepth(targetDepth));
    }
    

    private IEnumerator Coroutine_AnimateDepth(float targetDepth) {
        while (Depth != targetDepth) {
            Depth += (targetDepth-Depth) * depthEasing;
            if (Mathf.Approximately(Depth, targetDepth)) {
                Depth = targetDepth;
                break;
            }
            yield return null;
        }
    }
}
