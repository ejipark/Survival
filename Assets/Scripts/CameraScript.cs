using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraScript : MonoBehaviour
{
    // Private values
    private Vector3 offset;
    CinemachinePOV tpsPOV;
    public float rotateSpeed;

    [Header("Camera")]
    public Transform target;
    public Transform pivot;

    [Header("Player")]
    public PlayerScript player;
    public GameObject playerGun;

    [Header("Cinemachine")]
    public GameObject tpsCam;
    public GameObject tpsCanvas;
    public CinemachineVirtualCamera tpsVCam;

    [Header("PlayerUI")]
    public GameObject pauseMenu;
    public GameObject endMenu;
    public GameObject nextMenu;

    [Header("QuestUI")]
    public GameObject questUI;

    // Start is called before the first frame update
    void Start()
    {
        offset = target.position - transform.position;

        pivot.transform.position = target.transform.position;
        pivot.transform.parent = target.transform;

        tpsPOV = tpsVCam.GetCinemachineComponent<CinemachinePOV>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseMenu.activeSelf == true || endMenu.activeSelf == true || nextMenu.activeSelf == true || questUI.activeSelf == true || player.currHealth <= 0)
        {
            tpsPOV.m_HorizontalAxis.m_MaxSpeed = 0;
            tpsPOV.m_VerticalAxis.m_MaxSpeed = 0;
        }
        else
        {
            if (playerGun.activeSelf == false)
            {
                tpsCanvas.SetActive(false);
            }
            else
            {
                tpsCanvas.SetActive(true);
            }
            tpsPOV.m_HorizontalAxis.m_MaxSpeed = rotateSpeed;
            tpsPOV.m_VerticalAxis.m_MaxSpeed = rotateSpeed;
        }
    }

    void LateUpdate()
    {
        if (pauseMenu.activeSelf == false && endMenu.activeSelf == false && nextMenu.activeSelf == false && questUI.activeSelf == false && player.currHealth > 0)
        {
            float horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
            target.Rotate(0, horizontal, 0);

            float vertical = Input.GetAxis("Mouse Y") * rotateSpeed;
            pivot.Rotate(-vertical, 0, 0);

            float desiredXAngle = pivot.eulerAngles.x;
            float desiredYAngle = target.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(desiredXAngle, desiredYAngle, 0);
            transform.position = target.position - (rotation * offset);

            if (transform.position.y < target.position.y)
            {
                transform.position = new Vector3(transform.position.x, target.position.y, transform.position.z);
            }
            transform.LookAt(target);
        }
    }
}
