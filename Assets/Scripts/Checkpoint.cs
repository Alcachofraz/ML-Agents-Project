using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool visible = true;
    public Material transparentMaterial;
    public Material visibleMaterial;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().material = visible ? visibleMaterial : transparentMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
