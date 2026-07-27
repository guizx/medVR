using UnityEngine;
using UnityEngine.UI;

public class TeacherRandomSprite : MonoBehaviour
{
    [SerializeField] private Sprite[] teachers;
    [SerializeField] private Image teacherImage;

    private void OnEnable()
    {
        if(teachers.Length > 0)
        {            
            Sprite randomTeacher = teachers[Random.Range(0, teachers.Length)];
            teacherImage.sprite = randomTeacher;
        }
    }
}
