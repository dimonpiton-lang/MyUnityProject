using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class UpdateEventCamera : MonoBehaviour
{
    public Canvas targetCanvas;

    void Update()
    {
        // �������� �������� ����������� ������ Cinemachine
        // CinemachineVirtualCameraBase activeCamera = CinemachineCore.Instance.ActiveCamera as CinemachineVirtualCameraBase;  <- ���������� ���

        // ���������� CinemachineBrain ��� ��������� �������� ������
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null && brain.ActiveVirtualCamera != null)
        {
            // ������������� Event Camera �� ������, ������� ������������ Cinemachine
            targetCanvas.worldCamera = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<Camera>();
        }
        else
        {
            Debug.LogWarning("No active Cinemachine camera found.");
        }
    }
}