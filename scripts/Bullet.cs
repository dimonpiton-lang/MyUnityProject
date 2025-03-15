using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f; // ����� ����� ���� � ��������

    void Start()
    {
        // ���������� ���� ����� �������� �����
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // ���� ���� ����������� � �������
        if (other.gameObject.CompareTag("Player"))
        {
            // �������� ��������� �������� ������ (���� ����)
            //Health playerHealth = other.GetComponent<Health>();

            // ���� � ������ ���� ��������, ������� ����
            //if (playerHealth != null)
            //{
            //    playerHealth.TakeDamage(10f); // ������� 10 �����
            //}

            // ���������� ����, ���������� �� ��������� ����
            Destroy(gameObject, 0f);
        }
    }
}