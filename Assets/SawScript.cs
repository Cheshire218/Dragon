using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawScript : MonoBehaviour {

    //При попадании в другой коллайдер2д
    void OnTriggerEnter2D(Collider2D col)
    {
        // Если попали в объект с тэгом "Player"
        if (col.tag == "Player" && !col.isTrigger)
        {
            playerHealth ph = col.GetComponent<playerHealth>();
            if (ph != null)
            {
                ph.CoupDeGrace();
            }
        }
    }
}
