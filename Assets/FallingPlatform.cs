using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour {

    //поле для трансформа 
    private Transform thisTransform;
    private bool fall;

    public float delay = 2f;
    [SerializeField]
    private float currentDelay = 0;
    public float fallingSpeed = 1f;
    private float startPositionY;

    private void Start()
    {
        thisTransform = transform;
        fall = false;
        startPositionY = thisTransform.position.y;
        currentDelay = delay;
    }

    private void Update()
    {
        if(fall)
        {
            if (currentDelay <= 0)
            {
                thisTransform.Translate(Vector3.up * -1 * Time.deltaTime * fallingSpeed);
            }
            else
            {
                currentDelay -= Time.deltaTime;
            }
        }
        else 
        {
            if (thisTransform.position.y < startPositionY)
            {
                thisTransform.Translate(Vector3.up * Time.deltaTime * fallingSpeed);
            }
            else if (thisTransform.position.y > startPositionY)
            {
                thisTransform.position = new Vector3(thisTransform.position.x, startPositionY, thisTransform.position.z);
            }
            else
            {
                currentDelay = delay;
            }
        }
    }


    //При попадании в другой коллайдер2д
    void OnTriggerEnter2D(Collider2D col)
    {
        // Если попали в объект с тэгом "Player"
        if (col.tag == "Player" && !col.isTrigger)
        {
            fall = true;
        }
    }

    //При попадании в другой коллайдер2д
    void OnTriggerExit2D(Collider2D col)
    {
        // Если попали в объект с тэгом "Player"
        if (col.tag == "Player" && !col.isTrigger)
        {
            fall = false;
        }
    }
}
