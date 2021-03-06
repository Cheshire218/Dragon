﻿using UnityEngine;

public class enemyProjectile : MonoBehaviour {
    //Префаб взрыва снаряда
    public GameObject explosion;
    // поле, где будем хранить rigidBody2d, чтоб постоянно не дергать его
    private Rigidbody2D RB2D;
    //поле для трансформа (из тех же соображений, что и RB2D)
    private Transform thisTransform;

    //Закешируем некоторые компоненты и уничтожим объект спустя 10 секунд после его появления
    void Start()
    {
        // Уничтожим снаряд спустя 10 сек, после появления, чтоб не летел пока игра не выключится, если не врежется во что-нибудь
        Destroy(gameObject, 10);
        //кешируем rigidBody2d
        RB2D = GetComponent<Rigidbody2D>();
        //Кешируем transform
        thisTransform = transform;
    }

    // ГДЕ ДЕТОНАТОР?!?!?!?! метод, создающий префаб взрыва
    void OnExplode()
    {
        // Повернем gameobject под рандомным углом
        Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

        // Создадим gameObject взрыва в месте текущего объекта и под рандомным углом
        Instantiate(explosion, transform.position, randomRotation);
    }

    //При попадании в другой коллайдер2д
    void OnTriggerEnter2D(Collider2D col)
    {
        // Если попали в объект с тэгом "Player"
        if (col.tag == "Player" && !col.isTrigger)
        {
            // Ищем у объекта компонент playerHealth и вызываем метод GetDamage
            col.gameObject.GetComponent<playerHealth>().GetDamage(15);
            // Вызываем взрыв ))
            OnExplode();
            // Уничтожаем снаряд
            Destroy(gameObject);
        }
        // Проверяем проверку на попадания во все остальное, кроме самого себя, иначе, если будет не ловкая ситуация когда снаряды взрываются в момент выстрела :)
        else if (col.gameObject.tag != "Enemy" && !col.isTrigger)
        {
            // Взрыв
            OnExplode();
            // Уничтожаем снаряд
            Destroy(gameObject);
        }
    }
}
