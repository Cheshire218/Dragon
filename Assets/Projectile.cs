using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	//Префаб взрыва снаряда
	public GameObject explosion;
    // поле, где будем хранить rigidBody2d, чтоб постоянно не дергать его
	private Rigidbody2D RB2D;
    //поле для трансформа (из тех же соображений, что и RB2D)
	private Transform thisTransform;

	//Закешируем некоторые компоненты и уничтожим объект спустя 10 секунд после его появления
	void Start () {
		// Уничтожим снаряд спустя 10 сек, после появления, чтоб не летел пока игра не выключится, если не врежется во что-нибудь
		Destroy(gameObject, 10);

        //кешируем rigidBody2d
		RB2D = GetComponent<Rigidbody2D> ();
        //Кешируем transform
		thisTransform = transform;
	}

	//При попадании в другой коллайдер2д
	void OnTriggerEnter2D (Collider2D col) {
		// Если попали в объект с тэгом "Enemy"
		if(col.tag == "Enemy" && !col.isTrigger)
		{
			// Ищем у объекта компонент Enemy и вызываем метод Hurt
			col.gameObject.GetComponent<MyEnemy>().Hurt();
			// Вызываем взрыв ))
			OnExplode();
			// Уничтожаем снаряд
			Destroy (gameObject);
		}
		// Проверяем проверку на попадания во все остальное, кроме персонажа, иначе, если будет не ловкая ситуация когда снаряды взрываются в момент выстрела :)
		else if(col.gameObject.tag != "Player" && !col.isTrigger)
		{
			// Взрыв
			OnExplode();
			// Уничтожаем снаряд
			Destroy (gameObject);
		}
	}

	// ГДЕ ДЕТОНАТОР?!?!?!?! метод, создающий префаб взрыва
	void OnExplode()
	{
		// Повернем gameobject под рандомным углом
		Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

		// Создадим gameObject взрыва в месте текущего объекта и под рандомным углом
		Instantiate(explosion, transform.position, randomRotation);
	}

    //метод вызывается каждый кадр
	void Update() {
        //смотрим направление силы, приложенной к снаряду
		Vector3 velocity = RB2D.velocity;
        //ищем угол, чтобы развернуть снаряд в нужную сторону
		float angle = Mathf.Atan2 (velocity.y, velocity.x) * Mathf.Rad2Deg;
        //разворачиваем снаряд
		thisTransform.rotation = Quaternion.Euler (new Vector3(0, 0, angle));
	}
}
