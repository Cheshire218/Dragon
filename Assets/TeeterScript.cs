using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeeterScript : MonoBehaviour {

    Transform leftPart, rightPart, thisTransform;

    public bool left, right;

    public float maxDeltaY = 3f;
    public float minDeltaY = 0.3f;

    public float fallingSpeed = 1f;
    private float leftStartPositionY, rightStartPositionY;

    // Use this for initialization
    void Start () {
        left = false;
        right = false;
        thisTransform = transform;
        leftPart = thisTransform.Find("Left").transform;
        rightPart = thisTransform.Find("Right").transform;

        leftStartPositionY = leftPart.position.y;
        rightStartPositionY = rightPart.position.y;

    }
	
	// Update is called once per frame
	void Update () {
        if (left)
        {
            if (Mathf.Abs(leftPart.position.y - leftStartPositionY) < maxDeltaY)
            {
                leftPart.Translate(Vector3.up * -1 * Time.deltaTime * fallingSpeed);
                rightPart.Translate(Vector3.up * Time.deltaTime * fallingSpeed);
            }
        }
        else if (right)
        {
            if (Mathf.Abs(rightPart.position.y - rightStartPositionY) < maxDeltaY)
            {
                rightPart.Translate(Vector3.up * -1 * Time.deltaTime * fallingSpeed);
                leftPart.Translate(Vector3.up * Time.deltaTime * fallingSpeed);
            }
        }
        else
        {
            float leftDelta = leftPart.position.y - leftStartPositionY;
            float rightDelta = rightPart.position.y - rightStartPositionY;

            if (Mathf.Abs(leftDelta) > minDeltaY)
            {
                Vector3 dir = Vector3.up * Time.deltaTime * fallingSpeed;
                if (leftDelta > 0)
                {
                    dir *= -1;
                }
                leftPart.Translate(dir);
            }
            else if(leftPart.position.y != leftStartPositionY)
            {
                leftPart.position = new Vector3(leftPart.position.x, leftStartPositionY, leftPart.position.z);
            }

            if (Mathf.Abs(rightDelta) > minDeltaY)
            {
                Vector3 dir = Vector3.up * Time.deltaTime * fallingSpeed;
                if (rightDelta > 0)
                {
                    dir *= -1;
                }
                rightPart.Translate(dir);
            }
            else if(rightPart.position.y != rightStartPositionY)
            {
                rightPart.position = new Vector3(rightPart.position.x, rightStartPositionY, rightPart.position.z);
            }
        }
    }
}
