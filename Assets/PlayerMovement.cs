using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    //поле для хранения gameObject'а который будет создаваться. Другими словами для наших мин.
    public GameObject bomb;


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


    public Image bombImage;
    private float bombReloadMax = 1f;
    private float bombReloadCur;


    Vector2 touchMoveStartPoint;

    float horizontalInput;
    bool jump;

    


    // В этом методе закешируем необходимые данные
    void Start () {
        jump = false;
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

        bombReloadCur = 0;
    }

    //Метод расчитывае угол под которым полетит снаряд, в зависимости от положения мышки.
    private void Aiming(Touch currentTouch)
    {
        // рассчитаем позицию персонажа относительно экранной системы координат
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        //обнулим глубину

        Vector2 posi = new Vector2(pos.x, pos.y);

        Vector2 aimDir = currentTouch.position - posi;
        //расчитаем угол (в градусах) необходимый для прицеливания в мышку и запишем этот угол в поле angle
        angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

    }



    void TouchMoving()
    {
        //Если нажата кнопка прыжка и мы стоим на земле
        if (jump && grounded && !stopWalking)
        {
            //Прыгаем
            thisRB2D.AddForce(new Vector2(0f, jumpForce));
        }
        if (Input.touchCount > 0)
        {



            if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                Touch moveTouch = Input.touches[0];
                


                if (moveTouch.phase == TouchPhase.Began)
                {
                    touchMoveStartPoint = moveTouch.position;

                }


                Vector2 delta = moveTouch.position - touchMoveStartPoint;
                if (Mathf.Abs(delta.x) > Screen.width / 25)
                {
                    if (moveTouch.phase == TouchPhase.Moved || moveTouch.phase == TouchPhase.Stationary)
                    { 
                        delta = delta.normalized;
                        horizontalInput = delta.x;
                        if(Input.touchCount > 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(1).fingerId))
                        {
                            
                            Touch secondTouch = Input.touches[1];

                            if (secondTouch.phase == TouchPhase.Moved || secondTouch.phase == TouchPhase.Stationary)
                            {
                                //Вызываем метод Aiming, чтобы персонаж стрелял туда, куда смотрит мышка. Метод сохранит угол в поле angle
                                Aiming(secondTouch);
                            }

                            if (secondTouch.phase == TouchPhase.Ended)
                            {
                                if (grounded)
                                {
                                    stopWalking = true;
                                }
                                _animator.SetTrigger("breath_attack");
                            }
                        }   
                    }
                    
                }
                else if(moveTouch.phase == TouchPhase.Ended)
                {
                    Aiming(moveTouch);
                    if (grounded)
                    {
                        stopWalking = true;
                    }
                    _animator.SetTrigger("breath_attack");
                }
                else
                {
                    horizontalInput = 0;
                }

            }
           

        }
        else
        {
            touchMoveStartPoint = Vector2.zero;
            horizontalInput = 0;
        }

    }

    
    public void Jumping()
    {
        jump = true;
    }


    //Метод, который вызывается определенное количество раз в секунду
    void FixedUpdate() {
        //Проверяем стоит ли персонаж на земле или нет. Для этого посылаем луч из трансформ позиции персонажа до трансформ.позиции объекта groundCheck.
        //Конструкция 1 << LayerMask.NameToLayer("Ground") нужна чтобы указать побитовую маску слоя Ground = 1 * 2^12 или 0001 0000 0000 0000
        grounded = Physics2D.Linecast(thisTransform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        //анимация прыжка
        _animator.SetBool("jump", !grounded);


        

        //записываем в переменную использование элментов управления по оси Horizontal. Другими словами, мы назначили для горизонтальной оси кнопка влево - "A", вправо "D". Проверяем не нажаты ли эти кнопки
        //horizontalInput хранит в себе дробное число от -1 до 1. 0 - значит кнопки не нажаты, 1 - вправо, -1 - влево.
        //horizontalInput = Input.GetAxis ("Horizontal");


        TouchMoving();
        /*
            //Если нажата кнопка огня
            if (Input.GetButtonDown("Fire1"))
            {
                if (grounded)
                {
                    stopWalking = true;
                }
                _animator.SetTrigger("breath_attack");

            }

            //Если нажимаем кнопку "Fire2" (в данном случае пр.кнопка мыши или альт), то
            if (Input.GetButtonDown("Fire2") && bombReloadCur <= 0)
            {
                // создаем на сцене gameobject bomb, в позиции transform.position и с ротацией transform.rotation
                Instantiate(bomb, transform.position, transform.rotation);
                bombReloadCur = bombReloadMax;
            }
            else if (bombReloadCur > 0)
            {
                bombReloadCur -= Time.fixedDeltaTime;
                bombImage.fillAmount = (bombReloadMax - bombReloadCur) / bombReloadMax;
            }
        */
        jump = false;










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


        /*
        */

       



        
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
            thisRB2D.AddForce(new Vector2(100 * horInput * Time.fixedDeltaTime * speed, 0f), ForceMode2D.Force);
		}
        else
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
