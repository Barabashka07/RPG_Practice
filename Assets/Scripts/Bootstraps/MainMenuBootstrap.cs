using Controllers;
using Controllers.Scenes;
using Controllers.UI;
using UnityEngine;

namespace Bootstraps
{
    public class MainMenuBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            //Поиск кнопок, картинок от этих кнопок
            var viewManager = GetComponent<ViewManager>();
            //Переключение экранов через контроллер через манагер
            MainMenuManager menuManager = new(viewManager, GameManager.Instance.GetSettingsInteractor(),
                GameManager.Instance.GetPlayerDataInteractor());
                //Дебаг
            InvokeRepeating(nameof(Do),0f,1f);
        }

        private void Do() => print(GameManager.Instance.GetSettingsInteractor().LoadSettings().EnemiesPower);
        private void Update()
        {
            
        }
    }
}