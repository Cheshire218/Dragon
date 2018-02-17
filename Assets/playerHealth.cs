using UnityEngine;
using UnityEngine.UI;

//новый класс, для здоровья персонажа
public class playerHealth : MonoBehaviour {

    //текущий уровень здоровья
    private float health = 100f;
    //максимальный уровень здоровья
    private float maxHealth = 100f;
    //поле для Animator
    Animator _animator;
    //Время оглушения
    public float stunTime = 1f;
    //компонент управления
    PlayerMovement playerMove;


    //Храним риджид бади 2д, чтоб постоянно не обращаться к нему
    private Rigidbody2D thisRB2D;

    public Slider healthSlider;

	
    //метод получения дамага (далее сюда можно будет вкрутить броню, уменьшение урона и т.д.)
	public void GetDamage(float damageAmount)
    {
        //уменьшаем хп, на величину дамага, но не меньше нуля и не больше максимального (на случай если аптечку реализовать getDamage с отрицательным значением ...)
        health = Mathf.Clamp(health - damageAmount, 0, maxHealth);
        healthSlider.value = health;

        //playerMove.enabled = false;
        if (health <= 0)
        {
            Death();
            return;
        }

        //_animator.SetTrigger("stunned");
        
        //Invoke("StunFree", stunTime);
    }

    void StunFree()
    {
        _animator.SetTrigger("stun_free");
        playerMove.enabled = true;
        playerMove.StartWalking();
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
        playerMove = GetComponent<PlayerMovement>();
        health = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        //Кешируем RigidBody2D
        thisRB2D = GetComponent<Rigidbody2D>();
    }

    void Death()
    {
        _animator.SetTrigger("death");
        playerMove.enabled = false;
        this.enabled = false;
        thisRB2D.bodyType = UnityEngine.RigidbodyType2D.Static;
        //Destroy(gameObject, 1f);
        Invoke("ReloadLevel", 1f);
    }

    void ReloadLevel()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void CoupDeGrace()
    {
        Death();
    }

}
