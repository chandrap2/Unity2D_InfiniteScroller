using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundScript : MonoBehaviour
{
    public float scrollSpeed = 2;

    [SerializeField] private RawImage rawImg;

    // Start is called before the first frame update
    void Start()
    {
        //rawImg = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        rawImg.uvRect = new Rect(rawImg.uvRect.position + Vector2.right * scrollSpeed * Time.deltaTime, rawImg.uvRect.size);
    }
}
