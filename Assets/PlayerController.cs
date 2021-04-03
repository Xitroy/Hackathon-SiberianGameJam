using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    public float speed;
    private Rigidbody rb;
    private Vector3 velocityVector;
    public Camera cam;
    public bool freezY = true;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        velocityVector = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        velocityVector += cam.transform.rotation * Vector3.forward;
        if (Input.GetKey(KeyCode.A))
        velocityVector += cam.transform.rotation * Vector3.left;
        if (Input.GetKey(KeyCode.S))
        velocityVector += cam.transform.rotation * Vector3.back;
        if (Input.GetKey(KeyCode.D))
        velocityVector += cam.transform.rotation * Vector3.right;

        if (freezY)
        velocityVector.y = 0;

        if (velocityVector.magnitude>0)
        rb.velocity = velocityVector/velocityVector.magnitude * speed;
        else
        rb.velocity = velocityVector;
    }
}
