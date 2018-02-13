using UnityEngine;
using UnityEngine.UI;

public class ChestScript : MonoBehaviour {

    private bool isWin = false;

    public Canvas WinPanel;

    //При попадании в другой коллайдер2д
    void OnTriggerEnter2D(Collider2D col)
    {
        // Если попали в объект с тэгом "Player"
        if (col.tag == "Player" && !col.isTrigger)
        {
            Win();
        }
        
    }

    void Win()
    {
        WinPanel.enabled = true;
        Time.timeScale = 0;
    }



    void OnGUI()
    {
        if(isWin)
        {
            GUI.Box(new Rect(Screen.width/2 - 100, Screen.height/2 -70, 200, 140), "You win!");
            if (GUI.Button(new Rect(Screen.width / 2 - 90, Screen.height / 2 - 15, 180, 30), "Restart"))
            {
                Application.LoadLevel(Application.loadedLevel);
            }
        }
    }
}
