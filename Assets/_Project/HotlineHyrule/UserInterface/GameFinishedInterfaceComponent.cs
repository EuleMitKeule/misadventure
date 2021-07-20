using System;
using System.ComponentModel;
using TMPro;
using UnityEngine;

namespace HotlineHyrule.UserInterface
{
    public class GameFinishedInterfaceComponent : MonoBehaviour
    {
        TextMeshProUGUI TimeLabel { get; set; }
        
        void Start()
        {
            Locator.LevelComponent.GameFinished += OnGameFinished;
            
            var timeLabelObject = GameObject.Find("label_time");
            if (!timeLabelObject) return;

            TimeLabel = timeLabelObject.GetComponent<TextMeshProUGUI>();
        }

        void OnGameFinished(object sender, EventArgs e)
        {
            var elapsedTime = (int)Locator.GameComponent.ElapsedTime;
            var elapsedSeconds = elapsedTime % 60;
            var elapsedMinutes = elapsedTime / 60 % 60;
            var elapsedHours = elapsedTime / 60 / 60;

            TimeLabel.text =
                $"{elapsedHours.ToString("D2")}:{elapsedMinutes.ToString("D2")}:{elapsedSeconds.ToString("D2")}";
        }
    }
}