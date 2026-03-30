using System;
using UnityEngine;
using UnityEngine.UI;

namespace Views.Gameplay
{
    //Наследование от View
    public class PauseView : View
    {
        // Инкапсуляция: кнопки приватные, чтобы никто извне не изменил их визуал
        [SerializeField] private Button saveButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button loadLastButton;
        [SerializeField] private Button toLoadChooseButton;

        // MVC 
        // View не знает, КАК сохранять. Он просит Контроллер дать ему Action
        public void SetSaveButtonListener(Action callback)
        {
            // Вешаем переданную инструкцию на клик кнопки.
            saveButton.onClick.AddListener(callback.Invoke);
        }
        
        public void SetMainMenuButtonListener(Action callback)
        {
            mainMenuButton.onClick.AddListener(callback.Invoke);
        }
        
        public void SetLoadLastButtonListener(Action callback)
        {
            loadLastButton.onClick.AddListener(callback.Invoke);
        }
        
        public void SetToLoadChooseButtonListener(Action callback)
        {
            toLoadChooseButton.onClick.AddListener(callback.Invoke);
        }
    }
}