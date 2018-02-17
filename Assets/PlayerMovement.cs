using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerMovement : MonoBehaviour {

	//Если моделька изначально повернута право, запоминаем её состояние
	public bool facingRight = true;
	//скорость, с которой двигается персонаж, меняется в инспекторе
	public float speed = 5;

	//объект вектор2, чтобы постоянно его не создавать, хранит вектор2, который говорит куда и с какой скоростью будем двигаться
	private Vector2 dir = new Vector3 (0, 0);
	//создадим приватную переменную, где будем хранить transform, чтобы постоянно не обращаться к методу GetComponent или к свойству transform.
	private Transform thisTransform;
	//Булева переменная, которая говорит нам стоит персонаж на земле сейчас или нет.
    [SerializeField]
	private bool grounded = false;
	#region "прыжок";
	//Трансформ объекта groundCheck, куда будет посылать луч, для проверки, стоит ли персонаж на земле или нет.
	private Transform groundCheck;
	//Храним риджид бади 2д, чтоб постоянно не обращаться к нему
	private Rigidbody2D thisRB2D;
	//Сила прыжка
	public float jumpForce = 1000f;
    #endregion;
    //Поле для компонента аниматор
    private Animator _animator;
    private bool stopWalking;
    //Префаб снаряда
    public Rigidbody2D projectile;
    //Скорость снаряда
    public float projSpeed = 20f;
    //Переменная для кеширования компонента audioSource
    AudioSource shootSound;
    //угол атаки
    private float angle;
    //точка, откуда появляются снаряды
    private Transform gun;
    float horizontalInput;

    private float currentReload;
    public float gunReload = 1f;

    public float moveForce = 365f;
    public Image attackImage;

    // В этом методе закешируем необходимые данные
    void Start () {
        currentReload = 0;
        stopWalking = false;
        //кешируем аниматор
        _animator = GetComponent<Animator>();
		//Кешируем трансформ
		thisTransform = transform;
		//Ищет объект среди дочерних с именем groundCheck. Если нет таких, то null
		groundCheck = transform.Find("groundCheck");
		//Кешируем RigidBody2D
		thisRB2D = GetComponent<Rigidbody2D>();
        gun = transform.Find("Gun");
        //кешируем audioSource
        shootSound = gun.GetComponent<AudioSource>();
    }

    //Метод расчитывае угол под которым полетит снаряд, в зависимости от положения мышки.
    private void Aiming()
    {
        if(currentReload > 0)
        {
            currentReload -= Time.deltaTime;

                
                attackImage.fillAmount = (gunReload - currentReload) / gunReload;

        }
        int touchCnt = Input.touchCount;
        if(touchCnt > 0)
        {
            for(int i = 0; i < touchCnt; i++ )
            {
                Touch thisTouch = Input.GetTouch(i);
                if (!EventSystem.current.IsPointerOverGameObject(thisTouch.fingerId) && thisTouch.phase == TouchPhase.Began)
                {
                    // рассчитаем позицию персонажа относительно экранной системы координат
                    Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
                    //обнулим глубину
                    Vector2 posi = new Vector2(pos.x, pos.y);
                    Vector2 aimDir = thisTouch.position - posi;
                    //расчитаем угол (в градусах) необходимый для прицеливания в мышку и запишем этот угол в поле angle
                    angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

                    if (currentReload <= 0)
                    {
                        _animator.SetTrigger("breath_attack");
                        Invoke("ThrowFire", 0.09f);
                        currentReload = gunReload;
                    }

                    break;
                }
            }
        }
        
    }

    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Jump") && grounded)
        {
            thisRB2D.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Force);
        }
        //Проверяем стоит ли персонаж на земле или нет. Для этого посылаем луч из трансформ позиции персонажа до трансформ.позиции объекта groundCheck.
        //Конструкция 1 << LayerMask.NameToLayer("Ground") нужна чтобы указать побитовую маску слоя Ground = 1 * 2^12 или 0001 0000 0000 0000
        grounded = Physics2D.Linecast(thisTransform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        //анимация прыжка
        _animator.SetBool("jump", !grounded);

        Aiming();
    }


    //Метод, который вызывается определенное количество раз в секунду
    void FixedUpdate() {
        //записываем в переменную использование элментов управления по оси Horizontal. Другими словами, мы назначили для горизонтальной оси кнопка влево - "A", вправо "D". Проверяем не нажаты ли эти кнопки
        //horizontalInput хранит в себе дробное число от -1 до 1. 0 - значит кнопки не нажаты, 1 - вправо, -1 - влево.
        horizontalInput = CrossPlatformInputManager.GetAxis("Horizontal");

        //Вызовем метод передвижения и поворота персонажа
        Move(horizontalInput);
        

        //(Если мышь левее (>90 градусов по модулю) нашего персонажа и персонаж смотрит вправо) 
        //или
        //(если мышь правее(<90 градусов по модулю) нашего персонажа и персонаж смотрит влево)
        if ((Mathf.Abs(angle) > 90 && facingRight) || (Mathf.Abs(angle) < 90 && !facingRight))
        {
            //повернем персонажа
            Flip();
        }

        //ограничим угол стрельбы
        //для этого
        //если смотрим влево
        if (Mathf.Abs(angle) > 90)
        {
            // и вниз
            if (angle < 0)
            {
                //то ограничим сектором от -115 до -180
                angle = Mathf.Clamp(angle, -180, -115);
            }
            // и вверх
            else
            {
                //то ограничим сектором от 115 до 180
                angle = Mathf.Clamp(angle, 115, 180);
            }
        }
        //если персонаж смотрит вправо
        else
        {
            //то ограничим сектором от -75 до 75
            angle = Mathf.Clamp(angle, -75, 75);
        }


       



        
	}

    public void StartWalking()
    {
        stopWalking = false;
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
        if (facingRight)
        {
            //Создадим gameObject из префаба projectile на сцене у которого позиция на сцене и rotation будут такие же как и у текущего объекта. 
            bulletInstance = Instantiate(projectile, gun.position, Quaternion.Euler(new Vector3(0, 0, angle))) as Rigidbody2D;
        }
        else
        {
            //То же самое, что и выше, но в другую сторону смотрит наш снаряд
            bulletInstance = Instantiate(projectile, gun.position, Quaternion.Euler(new Vector3(0, 0, angle))) as Rigidbody2D;
        }
        //добавим силу для движения снаряда (не забываем нормализовать вектор направления)
        bulletInstance.velocity = bulletDir.normalized * projSpeed;
    }

    public void Flip() {
		//В переменной указываем, что смотрим в другую сторону (если было лево - станет право, если было право - станет лево)
		facingRight = !facingRight;
		//Получаем Vector3 для масштабирования текущего объекта
		Vector3 theScale = thisTransform.localScale;
		//Меняем знак у X вектора3, чтобы развернуть спрайт в другую сторону по оси X
		theScale.x *= -1;
		//Присваиваем полученный выше вектор3 нашему трансформу, чтобы вычисления, произведенные выше, не пропали втуне, а применились для объекта
		thisTransform.localScale = theScale;
	}

	//Метод, отвечающий за передвижение и поворот персонажа
	private void Move(float horInput) {
		//Если персонаж должен двигаться
		if (Mathf.Abs(horInput) > 0.7 && !stopWalking) {

            //Запустим анимацию
            _animator.SetBool("walk", true);

            //Переместим персонажа с помощью силы
            //thisRB2D.AddForce(new Vector2(100 * horInput * Time.fixedDeltaTime * speed, 0f), ForceMode2D.Force);

            if (horInput * thisRB2D.velocity.x < speed)
            {
                thisRB2D.AddForce(Vector2.right * horInput * moveForce * Time.fixedDeltaTime);
            }

            if (Mathf.Abs(thisRB2D.velocity.x) > speed)
            {
                // ... set the player's velocity to the maxSpeed in the x axis.
                thisRB2D.velocity = new Vector2(Mathf.Sign(thisRB2D.velocity.x) * speed, thisRB2D.velocity.y);
            }


        }
        else if(grounded)
        {
            //уберем движение по X
            thisRB2D.velocity = new Vector2(0, thisRB2D.velocity.y);
            //Остановим анимацию ходьбы
            _animator.SetBool("walk", false);
        }

		//Если нажата кнопка вправо И мы не смотрим вправо - вызовем метод поворота персонажа
		if (horInput > 0 && !facingRight) {
            //поворот
			Flip ();
			//Иначе, если нажата кнопка влево И мы смотрим вправо - вызовем метод поворота персонажа
		} else if (horInput < 0 && facingRight) {
            //поворот
            Flip();
		}
	}


}
