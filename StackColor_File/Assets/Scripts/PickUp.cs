using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] int value;
    [SerializeField] Color _PickUpColor;
    [SerializeField] Rigidbody pickUpRB;
    [SerializeField] Collider pickUpCollider;
    Renderer renderer;
    private void Start()
    {
        renderer = GetComponent<Renderer>();
        renderer.material.SetColor("_Color", _PickUpColor); 
    }

    private void OnEnable()
    {
        PlayerScript.kick += KicK;
    }

    private void OnDisable()
    {
        PlayerScript.kick -= KicK;
    }
    private void KicK(float forceSend)
    {
        transform.parent = null;
        pickUpCollider.enabled = true;
        pickUpRB.isKinematic = false;
        pickUpRB.AddForce(new Vector3(0, forceSend, forceSend));
    }

   

    public Color GetColor()
    {
        return _PickUpColor;
    }
}
