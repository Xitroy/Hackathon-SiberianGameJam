using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRules : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform waterBlock;
    public float roofHeight;
    private Transform Crown;
    private Transform exit;
    private Transform player;
    void Start()
    {
        // exit = GameObject.FindWithTag("Crown").transform;
    }

    // Update is called once per frame
    void Update()
    {
        // print(GameObject.FindWithTag("Crown"));
    }
}
