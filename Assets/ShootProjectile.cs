using UnityEngine;

public class ShootProjectile : MonoBehaviour {

	//Префаб снаряда
	public Rigidbody2D projectile;
	//Скорость снаряда
	public float speed = 20f;

	//Скрипт управления персонажем для того чтоб определить в какую сторону стрелять снарядом
	private PlayerMovement playerMovement;
	//создадим приватную переменную, где будем хранить transform, чтобы постоянно не обращаться к методу GetComponent или к свойству transform.
	private Transform thisTransform;
	//Переменная для кеширования компонента audioSource
	AudioSource shootSound;


    //аниматор
    private Animator _animator;

    //угол атаки
	private float angle;

	// В этом методе закешируем необходимые данные
	void Start () {
		//Кешируем трансформ
		thisTransform = transform;
		//Достаем трансформ верхнего объекта в иерархии
		playerMovement = thisTransform.root.GetComponent<PlayerMovement>();
        //Достаем аниматор верхнего объекта в иерархии
        _animator = thisTransform.root.GetComponent<Animator>();
        //кешируем audioSource
        shootSound = GetComponent<AudioSource>();
	}
	
	//Метод вызывается в каждом кадре
	void Update () {

        //Вызываем метод Aiming, чтобы персонаж стрелял туда, куда смотрит мышка. Метод сохранит угол в поле angle
		Aiming ();


        //(Если мышь левее (>90 градусов по модулю) нашего персонажа и персонаж смотрит вправо) 
        //или
        //(если мышь правее(<90 градусов по модулю) нашего персонажа и персонаж смотрит влево)
        if ((Mathf.Abs(angle) > 90 && playerMovement.facingRight) || (Mathf.Abs(angle) < 90 && !playerMovement.facingRight)) {
            //повернем персонажа
			playerMovement.Flip ();
		}


        //ограничим угол стрельбы
        //для этого
        //если смотрим влево
		if (Mathf.Abs (angle) > 90)
        {
            // и вниз
			if (angle < 0)
            {
                //то ограничим сектором от -115 до -180
				angle = Mathf.Clamp (angle, -180, -115);            
			}
            // и вверх
            else
            {
                //то ограничим сектором от 115 до 180
                angle = Mathf.Clamp (angle, 115, 180);
			}
		}
        //если персонаж смотрит вправо
        else
        {
            //то ограничим сектором от -75 до 75
            angle = Mathf.Clamp (angle, -75, 75);
		}


		//Если нажата кнопка огня
		if (Input.GetButtonDown ("Fire1")) {
            _animator.SetTrigger("breath_attack");
            //ThrowFire();

        }

	}

    void ThrowFire()
    {
        //Проигрываем звук выстрела, который задан в компоненте AudioSource
        shootSound.Play();


        //angle переводим в радианы и расчитаем косинус и синус угла. в x положим косинус (отношение прилежащего катета к гипотенузе), а в y sin (отношение противолежащего катета к гипотенузе)
        //получим вектор направления
        Vector2 bulletDir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        // создадим переменную куда положим созданный снаряд
        Rigidbody2D bulletInstance;
        //Если смотрим вправо
        if (playerMovement.facingRight)
        {
            //Создадим gameObject из префаба projectile на сцене у которого позиция на сцене и rotation будут такие же как и у текущего объекта. 
            bulletInstance = Instantiate(projectile, thisTransform.position, Quaternion.Euler(new Vector3(0, 0, angle))) as Rigidbody2D;
        }
        else
        {
            //То же самое, что и выше, но в другую сторону смотрит наш снаряд
            bulletInstance = Instantiate(projectile, thisTransform.position, Quaternion.Euler(new Vector3(0, 0, angle))) as Rigidbody2D;
        }
        //добавим силу для движения снаряда (не забываем нормализовать вектор направления)
        bulletInstance.velocity = bulletDir.normalized * speed;
    }



    //Метод расчитывае угол под которым полетит снаряд, в зависимости от положения мышки.
    private void Aiming() {
        // рассчитаем позицию персонажа относительно экранной системы координат
		Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        //обнулим глубину
		pos.z = 0;
        //найдем вектор направления от персонажа к мышке
		Vector3 aimDir = Input.mousePosition - pos;
        //расчитаем угол (в градусах) необходимый для прицеливания в мышку и запишем этот угол в поле angle
		angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

	}
}
