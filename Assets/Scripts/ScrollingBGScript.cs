using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBGScript : MonoBehaviour
{
    public List<Transform> BG_SpriteTransforms;
    public float scrollSpeed;
    public float spriteHalfWidth;
    public int numCols;

    private float cameraLeftBoundX, cameraRightBoundX;
    private float resetXOffset;

    // Start is called before the first frame update
    void Start()
    {
        Camera mainCam = Camera.main;
        cameraLeftBoundX = mainCam.transform.position.x - mainCam.orthographicSize * mainCam.aspect - spriteHalfWidth;
        cameraRightBoundX = mainCam.transform.position.x + mainCam.orthographicSize * mainCam.aspect + spriteHalfWidth;

        resetXOffset = numCols * spriteHalfWidth * 2;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform t in BG_SpriteTransforms) {
            if (t.position.x < cameraLeftBoundX)
                t.Translate(resetXOffset, 0, 0);
        }

        foreach (Transform t in BG_SpriteTransforms)
            t.transform.Translate(-scrollSpeed * Time.deltaTime, 0, 0);
    }
}
