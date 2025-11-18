using System;
using SeaLegs.Core;
using UnityEngine;

namespace SeaLegs
{
    public class Waves : MonoSingleton<Waves>
    {
        public float amplitude = 1f;
        public float length = 2f;
        public float speed = 1f;
        public float t = 0f;

        protected override void Init()
        {
            Debug.Log("Waves initted");
        }
        
        private void FixedUpdate()
        {
            t += Time.fixedDeltaTime * speed;
        }

        public float GetHeight(float x)
        {
            return amplitude * Mathf.Sin(x / length + t);
        }
    }
}
