using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AnimationsButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Button button;
    [SerializeField] float scaleIncrease = 1.1f;  // פרמטר המשתנה לקביעת קנה המידה החדש
    private Vector3 originalScale;

    // Start is called before the first frame update
    void Start()
    {
        // שמירת קנה המידה המקורי של הכפתור
        originalScale = button.transform.localScale;

        // הוספת מאזין לאירוע לחיצה על הכפתור
        button.onClick.AddListener(OnButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        // בדיקה אם הכפתור פעיל ובמצב נורמלי
        if (button.interactable)
        {
            // אפשר להוסיף כאן פעולות נוספות
        }
    }

    // פונקציה שתופעל כאשר הכפתור נלחץ
    void OnButtonClick()
    {
        Debug.Log("Button clicked!");
    }

    // פונקציה שתופעל כאשר העכבר עומד על הכפתור
    public void OnPointerEnter(PointerEventData eventData)
    {
        // הגדלת קנה המידה של הכפתור
        button.transform.localScale = originalScale * scaleIncrease;
    }

    // פונקציה שתופעל כאשר העכבר יוצא מהכפתור
    public void OnPointerExit(PointerEventData eventData)
    {
        // החזרת קנה המידה של הכפתור לקנה המידה המקורי
        button.transform.localScale = originalScale;
    }
}
