using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class BlockManagerScript : MonoBehaviour
{
    public float worldSpeed;
    public GameObject groundBlock;
    public GameObject colCollider;
    public int maxHeight;
    public int[] initHeightProb = new int[5];
    public TMP_Text scoreText;

    private Queue<GameObject> blockPool;
    private Queue<GameObject> columnPool;

    private LinkedList<List<GameObject>> blockGrid;
    private LinkedList<GameObject> columnGrid;
    private LinkedList<float> colX;

    private System.Random rand;

    private Camera mainCam;
    private Vector3 camPos;
    private float camHalfW, camHalfH;
    private float camLeftEdgeX, camBottomEdgeY;

    private int blockGridH, blockGridW;

    private int colsClearedCurrWave, totalColsCleared, currWave;
    private int[] cumulHeightDiffProb;

    // Start is called before the first frame update
    void Start()
    {
        rand = new();

        mainCam = Camera.main;

        camPos = mainCam.transform.position;
        
        camHalfH = mainCam.orthographicSize;
        blockGridH = Mathf.CeilToInt(camHalfH * 2) + 4;

        camHalfW = camHalfH * mainCam.aspect;
        blockGridW = Mathf.CeilToInt(camHalfW * 2) + 4;

        camLeftEdgeX = camPos.x - camHalfW;
        camBottomEdgeY = camPos.y - camHalfH;

        cumulHeightDiffProb = new int[100];
        InitCumulProbLookup();

        InitBlockPool(blockGridH * blockGridW);
        colCollider.GetComponent<BoxCollider2D>().size = new Vector2(1, maxHeight);
        InitColumnPool(blockGridW + 15);
        
        InitBlockGrid();

        colsClearedCurrWave = 0;
        currWave = 1;
    }

    private void Update() {
        LinkedListNode<List<GameObject>> currCol = blockGrid.First;
        LinkedListNode<GameObject> currColCollider = columnGrid.First;
        LinkedListNode<float> currColX = colX.First;

        float currDT = Time.deltaTime;

        // if a column moves out of the frame
        if (currColX.Value < camLeftEdgeX - 0.5) {
            UpdateScore();

            List<GameObject> currColVal = currCol.Value;
            GameObject currColColliderVal = currColCollider.Value;

            blockGrid.RemoveFirst();
            colX.RemoveFirst();
            colX.AddLast(colX.Last.Value + 1);
            columnGrid.RemoveFirst();

            // remove blocks and column collider from world
            RemoveBlocks(currColVal);
            RemoveColCollider(currColColliderVal);

            // adding new column to right
            //int prevHeight = blockGrid.Last.Value.Count;
            //int colH = math.clamp(rand.Next(prevHeight - 2, prevHeight + 3), 0, maxHeight);
            int colH = generateNextColumnHeight();

            UseBlocks(currColVal, colH);

            for (int j = 0; j < colH; j++) {
                currColVal[j].transform.Translate(colX.Last.Value, camBottomEdgeY + j + 0.5f, 0);
                currColVal[j].SetActive(true);
            }

            blockGrid.AddLast(currColVal);

            currColColliderVal = UseColCollider();
            currColColliderVal.transform.position = new Vector3(colX.Last.Value, camBottomEdgeY + (maxHeight / 2) - (maxHeight - colH) + 0.5f, 0);
            if (colH == 0)
                currColColliderVal.transform.Translate(Vector3.up * -15);
            currColColliderVal.SetActive(true);
            columnGrid.AddLast(currColColliderVal);

            currCol = blockGrid.First;
            currColCollider = columnGrid.First;
            currColX = colX.First;
        }

        for (int i = 0; i < blockGrid.Count; i++) {
            foreach (GameObject b in currCol.Value)
                b.transform.Translate(-worldSpeed * currDT, 0, 0);

            currColCollider.Value.transform.Translate(-worldSpeed * currDT, 0, 0);

            currColX.Value += -worldSpeed * currDT;

            currCol = currCol.Next;
            currColCollider = currColCollider.Next;
            currColX = currColX.Next;
        }
    }

    private void InitBlockGrid()
    {
        blockGrid = new();
        columnGrid = new();
        colX = new();

        for (int i = 0; i < blockGridW; i++)
        {
            // initializing ground blocks
            List<GameObject> col = new(blockGridH);

            //int colH;
            //if (i == 0) {
            //    colH = rand.Next(1, maxHeight);
            //} else {
            //    int prevHeight = blockGrid.Last.Value.Count;
            //    colH = math.clamp(rand.Next(prevHeight - 2, prevHeight + 3), 0, maxHeight);
            //}

            int colH = i == 0 ? rand.Next(1, maxHeight) : generateNextColumnHeight();

            for (int j = 0; j < colH; j++) {
                GameObject block = blockPool.Dequeue();
                block.transform.Translate(camLeftEdgeX + i, camBottomEdgeY + j + 0.5f, 0);
                block.SetActive(true);
                
                col.Add(block);
            }

            colX.AddLast(camLeftEdgeX + i);
            blockGrid.AddLast(col);

            // initializing column colliders
            GameObject currColCollider = UseColCollider();
            currColCollider.transform.position = new Vector3(camLeftEdgeX + i, camBottomEdgeY + (maxHeight / 2) - (maxHeight - colH) + 0.5f, 0);
            if (colH == 0)
                currColCollider.transform.Translate(Vector3.up * -15);
            columnGrid.AddLast(currColCollider);
            currColCollider.SetActive(true);
        }
    }

    private void InitBlockPool(int numBlocks) {
        blockPool = new(numBlocks);
        for (int i = 0; i < numBlocks; i++) {
            GameObject obj = Instantiate<GameObject>(groundBlock, Vector3.zero, Quaternion.identity);
            obj.SetActive(false);
            blockPool.Enqueue(obj);
        }
    }

    private void UseBlocks(List<GameObject> l, int num) {
        for (int i = 0; i < num; i++)
            l.Add(blockPool.Dequeue());
    }

    private void RemoveBlocks(List<GameObject> l) {
        foreach (GameObject b in l) {
            b.SetActive(false);
            b.transform.position = Vector3.zero;
            blockPool.Enqueue(b);
        }

        l.Clear();
    }

    private void InitColumnPool(int numCols) {
        columnPool = new(numCols);
        for (int i = 0; i < numCols; i++) {
            GameObject obj = Instantiate<GameObject>(colCollider, Vector3.up * -100 + Vector3.right * 150, Quaternion.identity);
            obj.SetActive(false);
            columnPool.Enqueue(obj);
        }
    }

    private GameObject UseColCollider() {
        return columnPool.Dequeue();
    }

    private void RemoveColCollider(GameObject obj) {
        obj.SetActive(false);
        obj.transform.position = Vector3.up * -100 + Vector3.right * 150;
        columnPool.Enqueue(obj);
    }

    private int generateNextColumnHeight() {
        int prevHeight = blockGrid.Last.Value.Count;
        if (prevHeight == 0) return 2;
        if (prevHeight == maxHeight) return maxHeight - 2;

        int nextHeightDiff = cumulHeightDiffProb[rand.Next(100)];

        return math.clamp(prevHeight + nextHeightDiff, 0, maxHeight);
    }

    private void InitCumulProbLookup() {
        int currLookupIndex = 0;
        for (int i = 0; i < 5; i++) {
            int diffProb = initHeightProb[i];

            for (int j = currLookupIndex; j < currLookupIndex + diffProb; j++)
                cumulHeightDiffProb[j] = -2 + i;

            currLookupIndex += diffProb;
        }
    }

    private void UpdateHeightDiffProb() {
        initHeightProb[0]++;
        initHeightProb[1]++;
        initHeightProb[2] -= 4;
        initHeightProb[3]++;
        initHeightProb[4]++;

        int currLookupIndex = 0;
        for (int i = 0; i < 5; i++) {
            int diffProb = initHeightProb[i];

            for (int j = currLookupIndex; j < currLookupIndex + diffProb; j++)
                cumulHeightDiffProb[j] = -2 + i;

            currLookupIndex += diffProb;
        }
    }

    private void UpdateScore() {
        scoreText.text = ++totalColsCleared + " m";

        if (++colsClearedCurrWave == blockGridW * 3) {
            colsClearedCurrWave = 0;
            currWave++;

            if (initHeightProb[2] > 10)
                UpdateHeightDiffProb();
            //else
            //    worldSpeed += 0.25f;
        }
    }
}
