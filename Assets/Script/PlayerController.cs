using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2D;
    public float moveSpeed = 5f;

    public float pullForce = 100f;
    public float rotateSpeed = 360f;
    private GameObject closestTower;
    private GameObject hookedTower;
    private bool isPulled = false;

    public Vector3 startPosition;

    private UIControllerScript uiControl;

    private AudioSource myAudio;
    private bool isCrashed = false;

    private RaycastHit2D hit;
    public LayerMask touchInputMask;
    GameObject recipent;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = this.gameObject.GetComponent<Rigidbody2D>();
        myAudio = this.gameObject.GetComponent<AudioSource>();
        uiControl = GameObject.Find("Canvas").GetComponent<UIControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        //Move the object
        rb2D.velocity = -transform.up * moveSpeed;

        if (Input.GetKey(KeyCode.Z) && !isPulled)
        {
            if (closestTower != null && hookedTower == null)
            {
                hookedTower = closestTower;
            }
            if (hookedTower)
            {
                float distance = Vector2.Distance(transform.position, hookedTower.transform.position);

                //Gravitation toward tower
                Vector3 pullDirection = (hookedTower.transform.position - transform.position).normalized;
                float newPullForce = Mathf.Clamp(pullForce / distance, 20, 50);
                rb2D.AddForce(pullDirection * newPullForce);

                //Angular velocity
                rb2D.angularVelocity = hookedTower.GetComponent<Tower>().positivity*rotateSpeed / distance;
                isPulled = true;
            }
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            isPulled = false;
            hookedTower = null;
            rb2D.angularVelocity = 0;
        }

        if (isCrashed)
        {
            if (!myAudio.isPlaying)
            {
                //Restart scene
                restartPosition();
            }
        }

        // fungsi pemilihan tower menggunakan mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = LayerMask.GetMask("Tower");
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), -Vector2.up, 1.5f, layerMask);
        if (hit.collider != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                recipent = hit.transform.gameObject;
                closestTower = recipent.gameObject;

                //Change tower color back to green as indicator
                recipent.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                closestTower = null;
                hookedTower = null;
                //Change tower color back to normal
                recipent.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }

    public void restartPosition()
    {
        //Set to start position
        this.transform.position = startPosition;

        //Restart rotation
        this.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

        //Set isCrashed to false
        isCrashed = false;

        if (closestTower)
        {
            closestTower.GetComponent<SpriteRenderer>().color = Color.white;
            closestTower = null;
        }

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            if (!isCrashed)
            {
                //Play SFX
                myAudio.Play();
                rb2D.velocity = new Vector3(0f, 0f, 0f);
                rb2D.angularVelocity = 0f;
                isCrashed = true;
            }
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            Debug.Log("Levelclear!");
            uiControl.endGame();
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        //Menonaktifkan pemilihan tower terdekat mengubah menjadi mouse pada Update
        
        /*if (collision.gameObject.tag == "Tower")
        {
            closestTower = collision.gameObject;

            collision.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        }
        */
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        //Menonaktifkan pemilihan tower terdekat mengubah menjadi mouse pada Update
        
        /*if (isPulled) return;
        
        if (collision.gameObject.tag == "Tower")
        {
            closestTower = null;
            hookedTower = null;
            //Change tower color back to normal
            collision.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
        */
    }
}
