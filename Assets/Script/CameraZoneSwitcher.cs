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
            // קבלת הרכיב CinemachineBrain מהמצלמה הראשית
            CinemachineBrain cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();

            // מעבר על כל המצלמות הווירטואליות בסצנה
            foreach (CinemachineVirtualCamera cam in FindObjectsOfType<CinemachineVirtualCamera>())
            {
                if (cam == virtualCamera)
                {
                    // העלאת העדיפות של המצלמה המתאימה לאזור
                    cam.Priority = 10; // Priority גבוהה יותר גורמת למצלמה זו להיות פעילה
                }
                else
                {
                    // הורדת העדיפות של שאר המצלמות
                    cam.Priority = 0; // Priority נמוכה יותר גורמת למצלמה זו להיות לא פעילה
                }
            }
        }
    }
}
