using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] Color _PickUpColor;
    Renderer renderer;
    private void Start()
    {
        renderer = GetComponent<Renderer>();
        renderer.material.SetColor("_Color", _PickUpColor); 
    }
    public Color GetColor()
    {
        return _PickUpColor;
    }
}
