using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class UpdateEventCamera : MonoBehaviour
{
    public Canvas targetCanvas;

    void Update()
    {
        // Получаем активную виртуальную камеру Cinemachine
        // CinemachineVirtualCameraBase activeCamera = CinemachineCore.Instance.ActiveCamera as CinemachineVirtualCameraBase;  <- Устаревший код

        // Используем CinemachineBrain для получения активной камеры
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null && brain.ActiveVirtualCamera != null)
        {
            // Устанавливаем Event Camera на камеру, которую контролирует Cinemachine
            targetCanvas.worldCamera = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<Camera>();
        }
        else
        {
            Debug.LogWarning("No active Cinemachine camera found.");
        }
    }
}