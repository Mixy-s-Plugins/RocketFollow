using SDG.Unturned;
using UnityEngine;

namespace RocketFollow
{
    public class MyRocket : MonoBehaviour
    {
        private float Speed { get; set; } 

        private float RotationSpeed { get; set; } 

        private float FocusDistance { get; set; } 

        private Transform Target { get; set; }

        private bool IsLookingAtObject { get; set; }

        public void Fire(Transform target, float speed, float rotationSpeed, float focusDistance)
        {
            Speed = speed;
            RotationSpeed = rotationSpeed;
            FocusDistance = focusDistance;
            Target = target;
            IsLookingAtObject = true;
        }

        private void Update()
        {
            var targetDirection = Target.position - transform.position;
 
            var newDirection = Vector3.RotateTowards(transform.forward, targetDirection, RotationSpeed * Time.deltaTime, 0.0F);
 
            transform.Translate(Vector3.forward * Time.deltaTime * Speed, Space.Self);
 
            if(Vector3.Distance(transform.position, Target.position) < FocusDistance)
            {
                IsLookingAtObject = false;
            }
 
            if(IsLookingAtObject)
            {
                transform.rotation = Quaternion.LookRotation(newDirection);
            }

            if (this != null)
            {
                EffectManager.sendEffectReliable(125, 5000, transform.position);
            }
        }
    }
}