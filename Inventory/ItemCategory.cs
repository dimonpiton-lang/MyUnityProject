using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemCategory
{
    None,                // ��� ��������� (����� �������������� ��� ���������, ������� ������ ���� ����� ������)
    PrimaryWeapon,       // �������� ������
    SecondaryWeapon,     // ��������������� ������
    MeleeWeapon,         // ������ �������� ���
    Gadget,              // �������
    Consumable,          // ����������
    ThrowableWeapon,     // ����������� ������
    Material             // ��������� (�� ������������ � UI, ������������ ��� ������)
}
