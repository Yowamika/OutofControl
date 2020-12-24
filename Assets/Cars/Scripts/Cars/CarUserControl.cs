using System;
using UnityEngine;


namespace Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use

        private float h, v, re, dash;
     
        public void SetHorizontal(float _h) { h = _h; }

        public void SetVertical(float _v) { v = _v; }

        public void SetRestart(float _re) { re = _re; }

        public void SetDash(float _dash) { dash = _dash; }

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            h = Input.GetAxis("Horizontal") ;
            v = Input.GetAxis("Vertical");
            re = Input.GetAxis("Fire2");

#if !MOBILE_INPUT

            dash = Input.GetAxis("Jump");
            m_Car.Move(h, v, v, dash);
            m_Car.Restart(re);
#else
            m_Car.Move(h, v, v, 0f);
#endif
            
        }
    }
}
