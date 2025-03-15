using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

public class CameraController : MonoBehaviour
{

    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

    public float rotationSpeed;

   

    public GameObject thirdPersonCam;
    

    public CameraStyle currentStyle;
    public enum CameraStyle
    {
        Basic,
        
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // switch styles
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchCameraStyle(CameraStyle.Basic);
        

        // rotate orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        // roate player object
       

     
    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        
        thirdPersonCam.SetActive(false);
     

        if (newStyle == CameraStyle.Basic) thirdPersonCam.SetActive(true);
       

        currentStyle = newStyle;
    }
}