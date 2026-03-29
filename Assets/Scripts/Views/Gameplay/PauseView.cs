using System;
using UnityEngine;
using UnityEngine.UI;

namespace Views.Gameplay
{
    //Наследование от View
    public class PauseView : View
    {
        [SerializeField] private Button saveButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button loadLastButton;
        [SerializeField] private Button toLoadChooseButton;

        // MVC 
        public void SetSaveButtonListener(Action callback)
        {
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