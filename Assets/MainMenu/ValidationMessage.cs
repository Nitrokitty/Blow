using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValidationMessage : MonoBehaviour {

    public string text = "ERROR";
    public GUIStyle style;
    public bool isEnabled = false;

    public float height = 30f;    
    public float yOffsetPercent = .2f;  
    public string padding = "  ";
    public bool wordWrap = false;
    public float widthPercent = 1f;

    float oheight;
    float oYOffsetPercent;
    string oPadding;
    bool oWordWrap;
    float oWidthPercent;

    RectTransform rectTrans;
    Rect rectangle;

    // Use this for initialization
    void Start () {
        rectTrans = GetComponent<RectTransform>();

        oheight = height;
        oYOffsetPercent = yOffsetPercent;
        oPadding = padding;
        oWordWrap = wordWrap;
        oWidthPercent = widthPercent;
    }
	
	// Update is called once per frame
	void OnGUI () {
        if (isEnabled)
        {
            var parentSize = rectTrans.sizeDelta;
            var bounds = RectTransformToScreenSpace(rectTrans);

            rectangle = new Rect(bounds.x + 1f , bounds.y + parentSize.y*yOffsetPercent, parentSize.x * widthPercent - 2f, height);
            style.wordWrap = wordWrap;
            GUI.Label(rectangle, padding + text, style);
        }
    }

    //http://answers.unity3d.com/questions/1013011/convert-recttransform-rect-to-screen-space.html
    Rect RectTransformToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        return new Rect((Vector2)transform.position - (size * 0.5f), size);
    }

    public void RestProperties(bool disable)
    {
        height = oheight;
        yOffsetPercent = oYOffsetPercent;
        padding = oPadding;
        wordWrap = oWordWrap;
        widthPercent = oWidthPercent;

        isEnabled = !disable;
    }
}
