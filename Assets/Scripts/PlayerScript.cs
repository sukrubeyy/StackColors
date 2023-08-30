using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] Renderer[] myrenderer;
    [SerializeField] Color myColor;
    [SerializeField] float forwardSpeed;
    [SerializeField] bool isPlaying;
    Rigidbody rb;
    [SerializeField] float xSpeed;
    [SerializeField] Transform stackPosition;
    Transform parentPickUp;
    public Queue<Transform> allStackObjects;

    bool isGameStart;
    [SerializeField] float forwardForce;
    [SerializeField] float upForce;
    private bool isFinish;
    public Status status = Status.None;
    public GameObject canvasObj;
    public Button retryButton;

    private void Start()
    {
        retryButton.onClick.AddListener(() => { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); });
        rb = GetComponent<Rigidbody>();
        SetColor(myColor);
        allStackObjects = new Queue<Transform>();
    }

    private void Update()
    {
        if (isPlaying && status is Status.PlayMode)
        {
            moveForward();
        }


        if (Input.GetMouseButton(0) && !isGameStart)
        {
            if (!isPlaying && !isFinish)
            {
                isPlaying = true;
                status = Status.PlayMode;
            }

            if (status is Status.PlayMode)
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
        if (Physics.Raycast(ray, out hit, 100))
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(hit.point.x, transform.position.y, transform.position.z), xSpeed * Time.deltaTime);
        }
    }

    public void moveForward()
    {
        rb.velocity = Vector3.forward * forwardSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("FinishLineEnd"))
        {
            rb.velocity = Vector3.zero;
            isPlaying = false;
            isFinish = true;
            ForceChildStacks();
            status = Status.UIMode;
            canvasObj.SetActive(true);
            Camera.main.GetComponent<CameraFollower>().SetTarget(allStackObjects.Dequeue());
        }
    }

    private void ForceChildStacks()
    {
        upForce = allStackObjects.Count * 2;
        forwardForce = allStackObjects.Count * 5;
        foreach (var stack in allStackObjects)
        {
            stack.parent = null;
            Rigidbody rbStack = stack.GetComponent<Rigidbody>();
            stack.GetComponent<BoxCollider>().enabled = true;
            rbStack.isKinematic = false;
            rbStack.AddForce(new Vector3(0, upForce, forwardForce), ForceMode.Impulse);
            upForce -= 2;
            forwardForce -= 5;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PickUp")
        {
            //Temas ettiğimiz objenin transformunu alıyoruz
            Transform otherTransform = other.transform;
            allStackObjects.Enqueue(otherTransform);
            if (myColor != other.GetComponent<PickUp>().GetColor())
            {
                //Destroy(other.gameObject);
                if (parentPickUp != null)
                {
                    if (parentPickUp.childCount >= 1)
                    {
                        parentPickUp.position -= Vector3.up * parentPickUp.GetChild(parentPickUp.childCount - 1).localScale.y;
                        //Destroy(parentPickUp.GetChild(parentPickUp.childCount - 1).gameObject);
                    }
                    else
                    {
                        //Destroy(parentPickUp.gameObject);
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

            if (parentPickUp == null)
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


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "ColorWall")
        {
            SetColor(other.GetComponent<ColorWall>().getColor());
        }
    }
}


public enum Status
{
    None,
    PlayMode,
    UIMode,
}