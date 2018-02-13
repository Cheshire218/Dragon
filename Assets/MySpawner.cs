using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySpawner : MonoBehaviour {

	//интервал повторения вызова метода Spawn
	public float spawnTime = 5f;
	//Через какое время запустим метод Spawn после вызова его с помощью InvokeRepeating
	public float spawnDelay = 3f;
	//Массив префабов врагов
	public GameObject[] enemies;
	//создадим приватную переменную, где будем хранить transform, чтобы постоянно не обращаться к методу GetComponent или к свойству transform.
	private Transform thisTransform;

    public int maxEnemies = 3;
    private int currentEnemies;

	// В этом методе закешируем необходимые данные и поставим на повтор метод Spawn
	void Start () {
        currentEnemies = 0;
		//Кешируем трансформ
		thisTransform = transform;
		// Вызываем метод Spawn через spawnDelay секунд и повторяем потом каждые spawnTime секунд
		InvokeRepeating("Spawn", spawnDelay, spawnTime);
	}

    public void EnemyDead()
    {
        currentEnemies = Mathf.Clamp(currentEnemies - 1, 0, maxEnemies);
    }

	//Метод спавна противников
	void Spawn ()
	{

        if (currentEnemies < maxEnemies)
        {
            // Достаем индекс рандомного противника из массива противников
            int enemyIndex = Random.Range(0, enemies.Length);

            Vector3 newPos = new Vector3(thisTransform.position.x + (Random.Range(-300, 300) / 100), thisTransform.position.y, thisTransform.position.z);

            // Создаем этого противника с позицией и направлением текущего gameObject
            GameObject newEnemy = Instantiate(enemies[enemyIndex], newPos, thisTransform.rotation);
            MyEnemy enemyBehaviour = newEnemy.GetComponent<MyEnemy>();
            enemyBehaviour.spawner = this;
            currentEnemies++;
        }
	}
}
