using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyTrailTouch : MonoBehaviour {

    private List<GameObject> trails;

    public GameObject trailRenderer;

    // Use this for initialization
    void Start () {
        trails = new List<GameObject>();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                {
                    if (Input.touches[i].phase == TouchPhase.Began)
                    {
                        if (trails.Count < i + 1)
                        {
                            Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(Input.touches[i].position.x, Input.touches[i].position.y, 0));
                            position.z = -2;
                            trails.Add(Instantiate(trailRenderer, position, transform.rotation));
                        }
                    }

                    if (Input.touches[i].phase == TouchPhase.Moved)
                    {
                        Vector3 curPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.touches[i].position.x, Input.touches[i].position.y, 0));
                        curPos.z = -2;
                        trails[i].transform.position = curPos;
                    }

                    if (Input.touches[i].phase == TouchPhase.Ended)
                    {
                        //Destroy(trails[i], 0.3f);
                        //trails.RemoveAt(i);
                    }
                }
            }
        }
    }
}
