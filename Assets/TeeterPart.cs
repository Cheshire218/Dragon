using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeeterPart : MonoBehaviour {

    TeeterScript root;
    public bool isLeft;

	void Start () {
        root = transform.root.GetComponent<TeeterScript>();
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player" && !col.isTrigger)
        {
            if (isLeft)
            {
                root.left = true;
            }
            else
            {
                root.right = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player" && !col.isTrigger)
        {
            if (isLeft)
            {
                root.left = false;
            }
            else
            {
                root.right = false;
            }
        }
    }
}
