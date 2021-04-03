using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnOf : MonoBehaviour
{
    // Start is called before the first frame update
    Light light;
    void Start()
    {
        light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            light.enabled = !light.enabled;
            Debug.Log("Pressed left click.");
        }
    }
}
