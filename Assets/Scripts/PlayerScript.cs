using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    #region Properties

    [SerializeField]
    private float speed = 1;

    private Rigidbody rb;
    private Vector3 movement;

    [SerializeField]
    private GameManager gameManager;

    #endregion

    #region Monobehavior Constructor

    /// <summary>
    /// Standard method of Monobehaviour Script that is called at loading of script
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    #endregion

    #region Monobehavior Methods

    /// <summary>
    /// Standard method of Monobehaviour Script that is called every frame
    /// </summary>
    void Update()
    {
        // get input without lag
        movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }

    /// <summary>
    /// Standard method of Monobehaviour Script that is called every consistent frame
    /// </summary>
    private void FixedUpdate()
    {
        // move rigidbody with physics
        rb.MovePosition(transform.position + (movement * speed * Time.deltaTime));
    }

    /// <summary>
    /// Standard method of Monobehaviour Script that is called when collision is detected
    /// </summary>
    void OnCollisionEnter(Collision col)
    {
        // if collide with last floor make player win
        if(col.gameObject.name == "EndFloor")
        {
            Debug.Log("You Win!");
            gameManager.Win();
        }
    }

    #endregion
}
