using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f; // Время жизни пули в секундах

    void Start()
    {
        // Уничтожаем пулю через заданное время
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Если пуля столкнулась с игроком
        if (other.gameObject.CompareTag("Player"))
        {
            // Получаем компонент здоровья игрока (если есть)
            //Health playerHealth = other.GetComponent<Health>();

            // Если у игрока есть здоровье, наносим урон
            //if (playerHealth != null)
            //{
            //    playerHealth.TakeDamage(10f); // Наносим 10 урона
            //}

            // Уничтожаем пулю, откладывая на следующий кадр
            Destroy(gameObject, 0f);
        }
    }
}