using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float scroll = Mathf.Repeat(Time.time * 0.05f, 1);
        Vector2 offset = new Vector2(-scroll, -scroll);
        GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);

    }
}
