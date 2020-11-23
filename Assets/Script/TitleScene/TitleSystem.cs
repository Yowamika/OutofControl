﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSystem : MonoBehaviour
{
    [SerializeField]
    Camera mainCamera;

    Loading loadingScript;
    // Start is called before the first frame update
    void Start()
    {
        loadingScript = mainCamera.GetComponent<Loading>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            loadingScript.NextScene();
        }
    }
}
