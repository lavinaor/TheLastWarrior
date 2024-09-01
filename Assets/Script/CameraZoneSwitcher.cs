using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZoneSwitcher : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ���� ����� CinemachineBrain ������� ������
            CinemachineBrain cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();

            // ���� �� �� ������� ������������ �����
            foreach (CinemachineVirtualCamera cam in FindObjectsOfType<CinemachineVirtualCamera>())
            {
                if (cam == virtualCamera)
                {
                    // ����� ������� �� ������ ������� �����
                    cam.Priority = 10; // Priority ����� ���� ����� ������ �� ����� �����
                }
                else
                {
                    // ����� ������� �� ��� �������
                    cam.Priority = 0; // Priority ����� ���� ����� ������ �� ����� �� �����
                }
            }
        }
    }
}
