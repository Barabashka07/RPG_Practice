using UnityEngine;
using UnityEngine.Events;

namespace Views
{
    // Абстрактный класс для всех экранов
    public abstract class View : MonoBehaviour
    {
        public void Show()
        {
            gameObject.SetActive(true);
            onShow.Invoke();
        }

        public UnityEvent onShow = new(); //Событие при закрытии окна
        public void Hide() => gameObject.SetActive(false);
        
    }
}