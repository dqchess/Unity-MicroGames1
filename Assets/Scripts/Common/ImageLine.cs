using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageLine : MonoBehaviour {
    // Components
    [SerializeField] private Image image=null; // the actual line
    [SerializeField] private RectTransform _myRectTransform=null;
    // Properties
    private float angle; // in DEGREES.
    private float length;
    private float thickness = 1f;
    // References
    private Vector2 startPos;
    private Vector2 endPos;

    // Getters
    public RectTransform rectTransform { get { return _myRectTransform; } }
    public float Angle { get { return angle; } }
    public float Length { get { return length; } }
    public Vector2 StartPos {
        get { return startPos; }
        set {
            if (startPos == value) { return; }
            startPos = value;
            UpdateAngleLengthPosition ();
        }
    }
    public Vector2 EndPos {
        get { return endPos; }
        set {
            if (endPos == value) { return; }
            endPos = value;
            UpdateAngleLengthPosition ();
        }
    }

    public void SetStartAndEndPos (Vector2 _startPos, Vector2 _endPos) {
        startPos = _startPos;
        endPos = _endPos;
        UpdateAngleLengthPosition ();
    }



    // ----------------------------------------------------------------
    //  Initialize
    // ----------------------------------------------------------------
    public void Initialize () {
        Initialize (Vector2.zero, Vector2.zero);
    }
    public void Initialize (Transform parentTransform) {
        Initialize (parentTransform, Vector2.zero, Vector2.zero);
    }
    public void Initialize (Vector2 _startPos, Vector2 _endPos) {
        Initialize (this.transform.parent, _startPos,_endPos);
    }
    public void Initialize (Transform _parentTransform, Vector2 _startPos, Vector2 _endPos) {
        startPos = _startPos;
        endPos = _endPos;

        GameUtils.ParentAndReset(this.gameObject, _parentTransform);

        UpdateAngleLengthPosition ();
    }


    // ----------------------------------------------------------------
    //  Update Things
    // ----------------------------------------------------------------
    private void UpdateAngleLengthPosition() {
        // Update values
        angle = -LineUtils.GetAngle_Degrees (startPos, endPos); // HACK why negative?
        length = LineUtils.GetLength (startPos, endPos);
        // Transform image!
        if (float.IsNaN (endPos.x)) {
            Debug.LogError ("Ahem! An ImageLine's endPos is NaN! (Its startPos is " + startPos + ".)");
        }
        rectTransform.anchoredPosition = LineUtils.GetCenterPos(startPos, endPos); //.transform.localPosition
        this.transform.localEulerAngles = new Vector3 (0, 0, angle);
        SetThickness (thickness);
    }

    public bool IsVisible {
        get { return image.enabled; }
        set {
            image.enabled = value;
        }
    }
    public void SetAlpha(float alpha) {
        GameUtils.SetUIGraphicAlpha(image, alpha);
    }
    public void SetColor(Color color) {
        image.color = color;
    }
    //  public void SetSortingOrder(int sortingOrder) {
    //      image.sortingOrder = sortingOrder;
    //  }
    public void SetThickness(float _thickness) {
        thickness = _thickness;
		GameUtils.SizeUIGraphic(image, length, thickness);
    }


}




