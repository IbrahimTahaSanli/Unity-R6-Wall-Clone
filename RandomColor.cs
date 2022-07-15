using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        this.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
