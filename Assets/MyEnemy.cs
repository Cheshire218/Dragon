using UnityEngine;

public class MyEnemy : MonoBehaviour {
    //Количество жизней
	public int HP = 2;
	//Спрайт при смерти
	public Sprite deadEnemy;
	//Спрайт при повреждении
	public Sprite damagedEnemy;
	//минимальный разворот при смерти
	public float deathSpinMin = -100f;
	//Максимальный разворот при смерти
	public float deathSpinMax = 100f;

	//ссылка на SpriteRenderer
	private SpriteRenderer ren;
	//проверка на смерть
	private bool dead = false;
    //трансформ игрока
    private Transform target;
    //скрипт здоровья игрока
    private playerHealth targetHealth;
    //Позиция куда будет возвращаться враг
    private Vector3 startPosition;
    //поле, куда закешируем трансформ нашего GameObject'а
    private Transform thisTransform;
    //агроМетка
    private bool agro;
    //направление взгляда
    public bool lookRight;
    //сюда закешируем rigidbody2d
    private Rigidbody2D thisRB2D;
    //здесь префаб снаряда
    public Rigidbody2D projectile;
    //минимальная дистанция, с которой можно производить атаку и скорость бега
    public float minDistance, speed = 1000;
    //скорость снаряда
    public float projectileSpeed = 10;
    //направление, куда смотрит враг
    Vector3 Dir = new Vector3(1, 0);
    //кулдаун атаки
    public float coolDown = 2f;
    //текущий кулдаун (если 0 - можно бить)
    private float currentCoolDown = 0f;
    private Animator _animator;
    public MySpawner spawner;

    public bool turret = true;

    private Transform gun;


    //метод установит начальную позицию, куда будем возвращаться
    private void SetStartPosition()
    {
        //устанавливаем позицию
        startPosition = thisTransform.position;
    }

    void Start () {
        //кешируем rigidbody2d
        thisRB2D = GetComponent<Rigidbody2D>();
        //кешируем траснформ
        thisTransform = transform;
        //через пол секунды установим стартовую позицию (что успел упасть)
        Invoke("SetStartPosition", 0.5f);
        _animator = GetComponent<Animator>();
        gun = thisTransform.Find("gun").transform;
    }

    void ThrowBullet()
    {
        if (target != null && targetHealth != null)
        {
            Rigidbody2D bulletInstance;
            Vector3 bulletDir = target.position - thisTransform.position;
            float angle = Mathf.Atan2(bulletDir.y, bulletDir.x) * Mathf.Rad2Deg;
            bulletInstance = Instantiate(projectile, gun.position, Quaternion.Euler(new Vector3(0, 0, angle - 13.5f))) as Rigidbody2D;
            //приложим силу к нашему снаряду
            bulletInstance.velocity = bulletDir.normalized * projectileSpeed;
        }
    }

    //метод нанесения урона
    void Attack()
    {
        if (turret)
        {
            //если смотрим вправо
            _animator.SetTrigger("attack");
            //снаряд смотрит направо
            Invoke("ThrowBullet", 0.5f);
        }
    }
	
    //метод вызывается определенное количество раз в секунду
	void FixedUpdate ()
	{
        //если нас сагрили
        if (agro)
        {
            //смотрим бежать вправо или влево
            float x = target.position.x - thisTransform.position.x;
            //если влево и мы смотрим вправо
            if(x<0 && lookRight)
            {
                //то повернемся
                Flip();
            }
            //иначе если смотрим влево и бежать нам вправо
            else if (x > 0 && !lookRight)
            {
                //опять таки, повернемся
                Flip();
            }
            
            //если подошли на расстояние атаки и цель можно атаковать (есть скрипт, отвечающий за хп цели)
            if (target != null && targetHealth != null)
            {
                //если нет кд на атаку
                if (currentCoolDown <= 0)
                {
                    //ставим кд
                    currentCoolDown = coolDown;
                    //и атакуем
                    Attack();
                }
                //иначе
                else
                {
                    //ждём кд
                    currentCoolDown -= Time.fixedDeltaTime;
                }
            }
            //если слишком далеко
            else if(!turret)
            {
                //силы в направлении нужном двигать будут нас
                thisRB2D.AddForce(Dir * Time.fixedDeltaTime * speed, ForceMode2D.Force);
            }
        }
        //если мы не сагренны
        
    }

    //в триггер попал
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //игрок
        if (collision.gameObject.layer == 9)
        {
            //то становим наш трансформ таргета (это будет игрок)
            target = collision.gameObject.transform;
            //также запомним в поле скрипт здоровья игрока, чтоб потом атаковать его
            targetHealth = target.GetComponent<playerHealth>();
            //агримся
            agro = true;
        }
    }

    //при выходе коллайдера из нашего триггера
    private void OnTriggerExit2D(Collider2D collision)
    {
        //если это игрок вышел
        if (collision.gameObject.layer == 9) // Если этот триггер игрок
        {
            //обнуляем таргет
            target = null;
            //и сбрасываем агро
            agro = false;
        }
    }

    //метод поворота
    void Flip()
    {
        //теперь смотрим в другую сторону
        lookRight = !lookRight;
        //векор направления тоже меняем
        Dir.x *= -1;
        // Берём новую структуру Vector3 и копируем её с нашего scale
        Vector3 V = transform.localScale;
        // И меняем её ось x в другую сторону
        V.x *= -1; 
        // Присваиваем свою структуру, так как напрямую к localScale.x нам не обратиться
        transform.localScale = V;
    }


    //получение дамага от игрока
    public void Hurt()
	{
		//уменьшаем хп на 1
		HP--;

        // Если хп отняли до 0 и ещё не указано, что враг умер
        if (HP <= 0 && !dead)
        { 
            // то умираем
           Death();
        }
    }

	//Помирает враг
	void Death()
	{
        //Ищем все SpriteRenderer в дочерних объектах
        _animator.SetTrigger("Death");
        thisRB2D.bodyType = RigidbodyType2D.Kinematic;


        //Ищем все Collider2D в дочерних объектах
        Collider2D[] otherColliders = GetComponentsInChildren<Collider2D>();
        // Всех их
        foreach (Collider2D s in otherColliders)
        {
            //вырубаем
            s.enabled = false;
        }

        
        

		//говорим что враг мертв
		dead = true;


		// Получаем все компоненты типа Collider2D
		Collider2D[] cols = GetComponents<Collider2D>();
		//Все эти коллайдеры
		foreach(Collider2D c in cols)
		{
			//делаем тригеррами
			c.isTrigger = true;
		}
        if (spawner != null)
        {
            spawner.EnemyDead();
        }
        //Уничтожаем gameObject
        Destroy(gameObject, 1.5f);
	}

}
