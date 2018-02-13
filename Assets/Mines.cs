using UnityEngine;

public class Mines : MonoBehaviour {

    //поле для хранения gameObject'а который будет создаваться. Другими словами для наших мин.
	public GameObject bomb;
	
	//Метод вызывается раз в кадр
	void Update () {
		//Если нажимаем кнопку "Fire2" (в данном случае пр.кнопка мыши или альт), то
		if (Input.GetButtonDown ("Fire2")) {
            // создаем на сцене gameobject bomb, в позиции transform.position и с ротацией transform.rotation
            Instantiate(bomb, transform.position, transform.rotation);
		}
	}
}
