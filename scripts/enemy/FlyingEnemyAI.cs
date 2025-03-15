using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FlyingEnemyAI : MonoBehaviour
{
    public Transform player;              // ������ �� ������
    public float moveSpeed = 5f;           // �������� ��������
    public float rotationSpeed = 5f;       // �������� ��������
    public float hoverHeight = 3f;         // �������� ������ ������
    public float upwardForce = 10f;        // ���� ��� ����������� ������
    public float obstacleAvoidanceDistance = 10f;  // ��������� ����������� �����������
    public float obstacleAvoidanceForce = 30f;    // ���� ��� ������ �����������
    public float playerAvoidanceDistance = 4f; // ��������� ������������ �� ������
    public LayerMask obstacleLayer;        // ���� �����������
    public float obstacleCheckHeight = 2f; // ������ �������� �����������
    public float attackRange = 5f;        // ��������� �����
    public float wanderStrength = 2f;      // ���� ��������� ����������
    public float sideRaycastAngle = 30f;  // ���� ������� �����
    public float searchRadius = 10f;       // ������ ������
    public float searchRotationSpeed = 50f; // �������� �������� � ������ ������
    public bool useRotation = true;          //  ������������ �������� � ������ ������
    public float smoothTime = 0.2f;        // ����� ����������� ��� avoidanceDirection

    private Rigidbody rb;
    private Vector3 avoidanceDirection; // ������� ����������� ������
    private Vector3 avoidanceDirectionVelocity; // �������� ��� SmoothDamp

    private Vector3 currentVelocity = Vector3.zero; // ������� �������� ��� SmoothDamp

    // ������ ���������
    private enum DroneState { Approaching, Attacking, Searching }
    private DroneState currentState = DroneState.Approaching;

    private float wanderAngle = 0f;       // ������� ���� ����������
    private float wanderChangeInterval = 2f; // ��� ����� ������ ���� ����������
    private float nextWanderChangeTime;

    private Vector3 lastKnownPlayerPosition; // ��������� ��������� ������� ������

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false; // ��������� ����������

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // �������������� ����� ���������� ��������� ���� ����������
        nextWanderChangeTime = Time.time + Random.Range(0f, wanderChangeInterval);
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // ����������� ������
        MaintainHeight();

        // ����� �����������
        avoidanceDirection = AvoidObstacles();

        // ��������� ��������� �����
        UpdateState();

        // ��������
        Move();

        //��������
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
            rb.AddForce(Vector3.down * upwardForce); // ������, ���� ��� �����
        }
    }

    Vector3 AvoidObstacles()
    {
        Vector3 newAvoidanceDirection = Vector3.zero;
        RaycastHit hit;

        // ��� ����� ����� ������
        bool forwardHit = Physics.Raycast(transform.position, transform.forward, out hit, obstacleAvoidanceDistance, obstacleLayer);

        if (forwardHit)
        {
            // ������ ��� ����������� �����������
            Debug.DrawRay(transform.position, transform.forward * obstacleAvoidanceDistance, Color.red);

            Vector3 obstacleTop = hit.point + Vector3.up * obstacleCheckHeight;
            if (!Physics.Raycast(obstacleTop, Vector3.down, out hit, obstacleCheckHeight * 2, obstacleLayer))
            {
                newAvoidanceDirection = Vector3.Cross(hit.normal, Vector3.up).normalized * obstacleAvoidanceForce;
                Debug.DrawRay(transform.position, newAvoidanceDirection * 2, Color.green);
            }
        }

        // ������� ��������� ����������� ������
        return Vector3.SmoothDamp(avoidanceDirection, newAvoidanceDirection, ref avoidanceDirectionVelocity, smoothTime);
    }

    bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer, obstacleLayer))
        {
            return false; // ����������� ����� ������ � �������
        }
        return true; // ����� �����
    }

    void UpdateState()
    {
        if (CanSeePlayer())
        {
            // ����� �����
            lastKnownPlayerPosition = player.position; // ��������� ��������� ��������� �������

            if (currentState == DroneState.Searching)
            {
                currentState = DroneState.Approaching; // ������������ �������������
            }

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (currentState == DroneState.Approaching && distanceToPlayer <= attackRange)
            {
                currentState = DroneState.Attacking;
            }
            else if (currentState == DroneState.Attacking && distanceToPlayer > attackRange * 1.2f) // ��������� ����������
            {
                currentState = DroneState.Approaching;
            }
        }
        else
        {
            // ����� �� �����
            if (currentState != DroneState.Searching)
            {
                currentState = DroneState.Searching; // ��������� � ����� ������
            }
        }
    }

    void Move()
    {
        Vector3 targetDirection = Vector3.zero;

        switch (currentState)
        {
            case DroneState.Approaching:
                // ��������� � ������
                targetDirection = (player.position - transform.position).normalized;

                //��������� ��������� ����������
                if (Time.time >= nextWanderChangeTime)
                {
                    wanderAngle = Random.Range(-1f, 1f) * wanderStrength;
                    nextWanderChangeTime = Time.time + Random.Range(0f, wanderChangeInterval);
                }

                // ��������� ���������� �� �����
                Quaternion rotation = Quaternion.AngleAxis(wanderAngle, Vector3.up);
                targetDirection = rotation * targetDirection;

                // ���������, ����� �� ������������� �� ������
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                if (distanceToPlayer < playerAvoidanceDistance)
                {
                    targetDirection = (transform.position - player.position).normalized; // ������������� �� ������
                }
                break;

            case DroneState.Attacking:
                //� ������ ����� ���� �� ���������
                break;

            case DroneState.Searching:
                // � ������ ������ ��������� � ��������� ��������� ������� ������
                targetDirection = (lastKnownPlayerPosition - transform.position).normalized;
                break;
        }

        // ��������� ����� �����������
        targetDirection += avoidanceDirection;
        targetDirection = targetDirection.normalized;

        // ����������� ��������
        Vector3 targetVelocity = targetDirection * moveSpeed;
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, 0.3f); // 0.3 - ����� �����������
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

    // ������������ � ���������
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, hoverHeight); // ������ ������

        Gizmos.color = Color.red;
        //Gizmos.DrawRay(transform.position, transform.forward * obstacleAvoidanceDistance); // ��������� ������ �����������

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerAvoidanceDistance); // ���������� ������������ �� ������

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange); // ��������� �����

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, searchRadius); // ������ ������
    }
}