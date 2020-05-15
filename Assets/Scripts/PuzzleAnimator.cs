using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleAnimator : MonoBehaviour
{
    public Vector3 block1Pos = new Vector3(14, 1, 12); // local origin in a puzzle soln
    public Vector3[] endPos = new Vector3[7]; // end local positions of 7 blocks
    public Vector3[] endRot = new Vector3[7]; // end orientations of 7 blocks in euler angle
    
    private int numBlocks = 7;
    private Vector3[] startPos = new Vector3[0];
    private Quaternion[] startRot = new Quaternion[0];
    private Quaternion[] endRotQuat = new Quaternion[0];

    // initialize start positions and orientations and
    // convert end orientation in euler angers to quaternions
    public void initialize()
    {
        startPos = new Vector3[numBlocks];
        startRot = new Quaternion[numBlocks];
        endRotQuat = new Quaternion[numBlocks];

        for (int i = 0; i < numBlocks; i++)
        {
            Transform child = this.gameObject.transform.GetChild(i);
            startPos[i] = child.position;
            startRot[i] = child.rotation;

            endPos[i] = block1Pos + endPos[i] * 0.5f; // scale relative pos bc Library(parent of puzzle objects) is scaled 
            endRotQuat[i] = Quaternion.Euler(endRot[i]);
        }
    }

    // getter methods
    public int getNumPieces()
    {
        return numBlocks;
    }

    public Vector3[] getStartPos()
    {
        return startPos;
    }

    public Vector3[] getEndPos()
    {
        return endPos;
    }

    public Quaternion[] getStartRot()
    {
        return startRot;
    }

    public Vector3[] getEndRot()
    {
        return endRot;
    }

    public Quaternion[] getEndRotQuat()
    {
        return endRotQuat;
    }
}
