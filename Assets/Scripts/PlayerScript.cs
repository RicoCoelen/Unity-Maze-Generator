using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField]
    private float speed = 1;

    private Rigidbody rb;
    private Vector3 movement;

    [SerializeField]
    private GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // get input without lag
        movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        // move rigidbody with physics
        rb.MovePosition(transform.position + (movement * speed * Time.deltaTime));
    }

    void OnCollisionEnter(Collision col)
    {
        // if collide with last floor make player win
        if(col.gameObject.name == "EndFloor")
        {
            Debug.Log("You Win!");
            gameManager.Win();
        }
    }
}
