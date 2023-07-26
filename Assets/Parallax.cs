using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startpos, groundlevel;
    public GameObject cam;
    public float parallaxEffect;
    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position.x;
        groundlevel = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);

        transform.position = new Vector3(startpos + dist, groundlevel, transform.position.z);

        /*if (temp > startpos + length +30) startpos += length;
        if (temp < startpos - length -30) startpos -= length;*/
    }
}
