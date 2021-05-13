using System;
using HotlineHyrule.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HotlineHyrule.Level
{
    public class TeleportComponent : MonoBehaviour
    {
        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("player")) return;

            Locator.GameComponent.LoadNextScene();
        }
    }
}