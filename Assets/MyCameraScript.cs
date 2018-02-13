using UnityEngine;

public class MyCameraScript : MonoBehaviour {

    //Если разница в X у камеры и персонажа больше числа в этом поле, то начнем двигать камеру
    public float xMargin = 1f;
    //Если разница в Y у камеры и персонажа больше числа в этом поле, то начнем двигать камеру
    public float yMargin = 1f;

    //чем ниже значение, тем плавнее (медленнее) будет двигаться камера, большое значение заставит камеру двигаться рывками
    //сглаживание по оси X
    public float xSmooth = 4f;
    //сглаживание по оси Y
    public float ySmooth = 4f;

    //сюда положим трансформ игрока, за которым будем следовать
    private Transform player;
    //здесь закешируем наш трансформ, чтоб постоянно к нему не стучаться
    private Transform thisTransform;

    //закешируем данные
    void Start () {
        //трансформ gameObject'а с тэгом "Player"
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //закешируем свой трансформ
        thisTransform = transform;
    }
	
	//метод вызывается определенное количество раз в секунду
	void FixedUpdate () {
        //вызовем метод слежения за персонажем
        Tracking();
	}

    //метод слежения за персонажем
    void Tracking()
    {
        //создадим новые коорданаты камеры с текущими данными 
        //по иксу
        float newX = thisTransform.position.x;
        //и по игреку
        float newY = thisTransform.position.y;

        //проверим отдалился ли персонаж по оси икс на расстояние, необходимое для движения камеры
        if (Mathf.Abs(thisTransform.position.x - player.position.x) > xMargin)
        {
            //используем линейную интерполяцию для более гладкого движения камеры
            newX = Mathf.Lerp(thisTransform.position.x, player.position.x, xSmooth * Time.fixedDeltaTime);
        }

        //проверим отдалился ли персонаж по оси игрек на расстояние, необходимое для движения камеры
        if (Mathf.Abs(thisTransform.position.y - player.position.y) > yMargin)
        {
            //используем линейную интерполяцию для более гладкого движения камеры
            newY = Mathf.Lerp(thisTransform.position.y, player.position.y, ySmooth * Time.fixedDeltaTime);
        }

        //меняем положение камеры в соответствии с новыми значениями
        thisTransform.position = new Vector3(newX, newY, thisTransform.position.z);
    }
}
