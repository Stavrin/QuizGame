using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{

    private Canvas canvassy;
    public Camera uiCam;

    // Start is called before the first frame update
    void Start()
    {

        canvassy= GetComponent<Canvas>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndGame()
    {
        canvassy.renderMode = RenderMode.ScreenSpaceCamera;
        canvassy.worldCamera= uiCam;
    }
}
