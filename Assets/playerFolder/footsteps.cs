using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class footsteps : MonoBehaviour
{
    // Start is called before the first frame update
    private bool stepsOnWater = false;
    private Rigidbody rb;
    AudioSource audio;
    void Start()
    {
        audio = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!stepsOnWater && rb.velocity.magnitude>0 && !audio.isPlaying) 
        audio.Play();
    }
}
