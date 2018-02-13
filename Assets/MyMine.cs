using UnityEngine;

public class MyMine : MonoBehaviour {

	//радиус поражения мины
	public float bombRadius = 10f;
    //сила, с которой мины толкнет всех в округе
	public float bombForce = 100f;
    //звук взрыва
	public AudioClip boom;
    //эффект взрыва, который останется после взрыва мины
	public GameObject explosion;

    public float maxLifeTime = 30f;

    //если что-то есть в триггере
    void OnTriggerStay2D(Collider2D other){
        //и это что-то "враг"
		if (other.tag == "Enemy" && !other.isTrigger) {
            //взрываем
			Explode ();
		}
	}

    private void Start()
    {
        Invoke("Explode", maxLifeTime);
    }

    //метод, который взрывает всё и вся! (отнимает 1хп, если будем отнимать больше - не увидим толчка от взрыва)
    public void Explode()
	{
		//ищем все коллайдеры в зоне поражения по маске
		Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, bombRadius, 1 << LayerMask.NameToLayer("Enemies"));


		//для каждого коллайдера врага
		foreach(Collider2D en in enemies)
		{
			//достаем его RigidBody2D
			Rigidbody2D rb = en.GetComponent<Rigidbody2D>();
            MyEnemy enemyBehaviour = rb.gameObject.GetComponent<MyEnemy>();

            //Если у врага есть компонент RigidBody2D и тэг у gameobject "Enemy"
            if (rb != null && rb.tag == "Enemy" && enemyBehaviour != null && !en.isTrigger)
			{
                //получим компонент MyEnemy этого врага и вызовем мето Hurt  (ouch ...)
                enemyBehaviour.Hurt ();
				//Расчитаем направление, в котором будем толкать взрывной волной
				Vector3 deltaPos = rb.transform.position - transform.position;
				//нормализуем вектор направления и придаем силу
				Vector3 force = deltaPos.normalized * bombForce * 20;
                //Гоблин - птица гордая: пока не пнёшь, не полетит! (c)    другими словами: толкаем врага взрывной волной
				rb.AddForce(force);
			}
		}

		//создаем gameobject, отвечающий за эффект взрыва
		Instantiate(explosion,transform.position, Quaternion.identity);

		//проигрываем звук взрыва
		AudioSource.PlayClipAtPoint(boom, transform.position);

		//Уничтожаем gameobject немедленно
		Destroy (gameObject);
	}
}
