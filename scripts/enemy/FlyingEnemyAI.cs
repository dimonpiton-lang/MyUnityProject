using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FlyingEnemyAI : MonoBehaviour
{
    public Transform player;              // Ссылка на игрока
    public float moveSpeed = 5f;           // Скорость движения
    public float rotationSpeed = 5f;       // Скорость поворота
    public float hoverHeight = 3f;         // Желаемая высота полета
    public float upwardForce = 10f;        // Сила для поддержания высоты
    public float obstacleAvoidanceDistance = 10f;  // Дистанция обнаружения препятствий
    public float obstacleAvoidanceForce = 30f;    // Сила для обхода препятствий
    public float playerAvoidanceDistance = 4f; // Дистанция отталкивания от игрока
    public LayerMask obstacleLayer;        // Слой препятствий
    public float obstacleCheckHeight = 2f; // Высота проверки препятствий
    public float attackRange = 5f;        // Дальность атаки
    public float wanderStrength = 2f;      // Сила случайных отклонений
    public float sideRaycastAngle = 30f;  // Угол боковых лучей
    public float searchRadius = 10f;       // Радиус поиска
    public float searchRotationSpeed = 50f; // Скорость вращения в режиме поиска
    public bool useRotation = true;          //  Использовать вращение в режиме поиска
    public float smoothTime = 0.2f;        // Время сглаживания для avoidanceDirection

    private Rigidbody rb;
    private Vector3 avoidanceDirection; // Текущее направление обхода
    private Vector3 avoidanceDirectionVelocity; // Скорость для SmoothDamp

    private Vector3 currentVelocity = Vector3.zero; // Текущая скорость для SmoothDamp

    // Режимы поведения
    private enum DroneState { Approaching, Attacking, Searching }
    private DroneState currentState = DroneState.Approaching;

    private float wanderAngle = 0f;       // Текущий угол отклонения
    private float wanderChangeInterval = 2f; // Как часто менять угол отклонения
    private float nextWanderChangeTime;

    private Vector3 lastKnownPlayerPosition; // Последняя известная позиция игрока

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false; // Отключаем гравитацию

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // Инициализируем время следующего изменения угла отклонения
        nextWanderChangeTime = Time.time + Random.Range(0f, wanderChangeInterval);
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Поддержание высоты
        MaintainHeight();

        // Обход препятствий
        avoidanceDirection = AvoidObstacles();

        // Обновляем состояние дрона
        UpdateState();

        // Движение
        Move();

        //Вращение
        Rotate();
    }

    void MaintainHeight()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f))
        {
            float heightDifference = hoverHeight - hit.distance;
            float upwardForceToApply = heightDifference * upwardForce;
            rb.AddForce(Vector3.up * upwardForceToApply);
        }
        else
        {
            rb.AddForce(Vector3.down * upwardForce); // Падаем, если нет земли
        }
    }

    Vector3 AvoidObstacles()
    {
        Vector3 newAvoidanceDirection = Vector3.zero;
        RaycastHit hit;

        // Луч прямо перед дроном
        bool forwardHit = Physics.Raycast(transform.position, transform.forward, out hit, obstacleAvoidanceDistance, obstacleLayer);

        if (forwardHit)
        {
            // Рисуем луч обнаружения препятствия
            Debug.DrawRay(transform.position, transform.forward * obstacleAvoidanceDistance, Color.red);

            Vector3 obstacleTop = hit.point + Vector3.up * obstacleCheckHeight;
            if (!Physics.Raycast(obstacleTop, Vector3.down, out hit, obstacleCheckHeight * 2, obstacleLayer))
            {
                newAvoidanceDirection = Vector3.Cross(hit.normal, Vector3.up).normalized * obstacleAvoidanceForce;
                Debug.DrawRay(transform.position, newAvoidanceDirection * 2, Color.green);
            }
        }

        // Плавное изменение направления обхода
        return Vector3.SmoothDamp(avoidanceDirection, newAvoidanceDirection, ref avoidanceDirectionVelocity, smoothTime);
    }

    bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer, obstacleLayer))
        {
            return false; // Препятствие между дроном и игроком
        }
        return true; // Игрок виден
    }

    void UpdateState()
    {
        if (CanSeePlayer())
        {
            // Игрок виден
            lastKnownPlayerPosition = player.position; // Обновляем последнюю известную позицию

            if (currentState == DroneState.Searching)
            {
                currentState = DroneState.Approaching; // Возобновляем преследование
            }

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (currentState == DroneState.Approaching && distanceToPlayer <= attackRange)
            {
                currentState = DroneState.Attacking;
            }
            else if (currentState == DroneState.Attacking && distanceToPlayer > attackRange * 1.2f) // Небольшой гистерезис
            {
                currentState = DroneState.Approaching;
            }
        }
        else
        {
            // Игрок не виден
            if (currentState != DroneState.Searching)
            {
                currentState = DroneState.Searching; // Переходим в режим поиска
            }
        }
    }

    void Move()
    {
        Vector3 targetDirection = Vector3.zero;

        switch (currentState)
        {
            case DroneState.Approaching:
                // Двигаемся к игроку
                targetDirection = (player.position - transform.position).normalized;

                //Добавляем случайные отклонения
                if (Time.time >= nextWanderChangeTime)
                {
                    wanderAngle = Random.Range(-1f, 1f) * wanderStrength;
                    nextWanderChangeTime = Time.time + Random.Range(0f, wanderChangeInterval);
                }

                // Вычисляем отклонение от курса
                Quaternion rotation = Quaternion.AngleAxis(wanderAngle, Vector3.up);
                targetDirection = rotation * targetDirection;

                // Проверяем, нужно ли отталкиваться от игрока
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                if (distanceToPlayer < playerAvoidanceDistance)
                {
                    targetDirection = (transform.position - player.position).normalized; // Отталкиваемся от игрока
                }
                break;

            case DroneState.Attacking:
                //В режиме атаки дрон не двигается
                break;

            case DroneState.Searching:
                // В режиме поиска двигаемся к последней известной позиции игрока
                targetDirection = (lastKnownPlayerPosition - transform.position).normalized;
                break;
        }

        // Добавляем обход препятствий
        targetDirection += avoidanceDirection;
        targetDirection = targetDirection.normalized;

        // Сглаживание движения
        Vector3 targetVelocity = targetDirection * moveSpeed;
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, 0.3f); // 0.3 - время сглаживания
    }

    void Rotate()
    {

        if (CanSeePlayer() && currentState != DroneState.Searching)
        {
            Vector3 targetDirection = player.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        }
        else if (currentState == DroneState.Searching && useRotation)
        {
            transform.Rotate(Vector3.up, searchRotationSpeed * Time.deltaTime);
        }
    }

    // Визуализация в редакторе
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, hoverHeight); // Высота полета

        Gizmos.color = Color.red;
        //Gizmos.DrawRay(transform.position, transform.forward * obstacleAvoidanceDistance); // Дистанция обхода препятствий

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerAvoidanceDistance); // Расстояние отталкивания от игрока

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Дальность атаки

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, searchRadius); // Радиус поиска
    }
}