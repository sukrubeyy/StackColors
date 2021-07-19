using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorWall : MonoBehaviour
{
    [SerializeField] Color MyColor;

    private void Start()
    {
        Color tempColor = MyColor;
        tempColor.a = .5f;
        Renderer rend = transform.GetChild(0).GetComponent<Renderer>();
        rend.material.SetColor("_Color", tempColor);
    }

    public Color getColor()
    {
        return MyColor;
    }
}
