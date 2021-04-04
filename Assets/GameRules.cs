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
        exit = GameObject.FindGameObjectsWithTag("Finish")[0].transform;
        print(exit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
