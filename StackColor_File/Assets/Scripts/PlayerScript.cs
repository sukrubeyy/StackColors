using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] Renderer[] myrenderer;
    [SerializeField] Color myColor;
    [SerializeField] float mySpeed;
    [SerializeField] bool isPlaying;
    Rigidbody rb;
    [SerializeField] float xSpeed;
    [SerializeField] Transform stackPosition;
    Transform parentPickUp;

    bool atEnd;
    [SerializeField] float forwardForce;
    [SerializeField] float forceAdder;
    [SerializeField] float forceReducer;
    public static Action<float> kick;


   
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetColor(myColor);
    }
    private void Update()
    {
        if(isPlaying)
        {
            moveForward();
        }
       

        if(atEnd)
        {
            forwardForce -= forceReducer * Time.deltaTime;
            if (forwardForce < 0)
                forwardForce = 0;
        }

        if(Input.GetMouseButtonDown(0) && atEnd)
        {
            forwardForce += forceAdder;
        }



        if (Input.GetMouseButton(0) && !atEnd)
        {
            if (!isPlaying)
            {
                isPlaying = true;
            }
            MoveSideways();
        }
    }
    public void SetColor(Color color)
    {
        myColor = color;
        for (int i = 0; i < myrenderer.Length; i++)
        {
            myrenderer[i].material.SetColor("_Color", myColor);
        }
    }
    public void MoveSideways()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,100))
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(hit.point.x, transform.position.y, transform.position.z), xSpeed * Time.deltaTime);
        }
    }
    public void moveForward()
    {
        rb.velocity = Vector3.forward * mySpeed;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="FinishLineEnd")
        {
            rb.velocity = Vector3.zero;
            isPlaying = false;
            LaunchStack();
        }

        if(other.tag=="FinishLineStart")
        {
            atEnd = true;
        }

        if (atEnd)
            return;

        if(other.tag=="PickUp")
        {
            //Temas ettiğimiz objenin transformunu alıyoruz
            Transform otherTransform = other.transform;
            if (myColor != other.GetComponent<PickUp>().GetColor())
            {
                Destroy(other.gameObject);
                if (parentPickUp != null)
                {
                    if (parentPickUp.childCount >= 1)
                    {
                        parentPickUp.position -= Vector3.up * parentPickUp.GetChild(parentPickUp.childCount - 1).localScale.y;
                        Destroy(parentPickUp.GetChild(parentPickUp.childCount - 1).gameObject);
                    }
                    else
                    {
                        Destroy(parentPickUp.gameObject);
                    }
                }
                return;
            }


            //temas ettiğimiz objenin rigidbody'sini alıyoruz
            Rigidbody otherRb = otherTransform.GetComponent<Rigidbody>();

          
            //fizik olaylarından muaf olmasını sağlarız
            otherRb.isKinematic = true;
            //etkileşim ettiğimiz objeyi pasif hale getiririz
            other.enabled = false;

            if(parentPickUp==null)
            {
                //parentPickUp objemiz sürekli değişecek bu yüzden etkileşime geçtiğimiz kutunun transformunu pickUp transformuna
                //atıyoruz
                parentPickUp = otherTransform;
                //parentPickUp pozisyonunu stack yani sıralama yaptığımız pozisyona eşitliyoruz
                parentPickUp.position = stackPosition.position;
                //Aldığımız obje StackPosition objesinin child objesi oluyor
                parentPickUp.parent = stackPosition;
            }
            else
            {
                
                parentPickUp.position += Vector3.up * (otherTransform.localScale.y);
                //Hali hazırda veya önceden aldığın pickUp objesi var ise bu objenin transformunu stackposition'a eşitleyelim
                otherTransform.position = stackPosition.position;
                //Daha sonra son alınan pickup objesni othertransform'a atayalım ki sıralama yaparken son alınan pickup
                //objesinin bir üzerine atama yapsın
                otherTransform.parent = parentPickUp;
            }
        }
    }

    private void LaunchStack()
    {
        Camera.main.GetComponent<CameraFollower>().SetTarget(parentPickUp);
        kick(forwardForce);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag=="ColorWall")
        {
            SetColor(other.GetComponent<ColorWall>().getColor());
        }
    }
}
