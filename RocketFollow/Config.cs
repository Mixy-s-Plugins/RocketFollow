using Rocket.API;

namespace RocketFollow
{
    public class Config : IRocketPluginConfiguration
    {
        public float Speed;
        
        public float RotationSpeed;
        
        public float FollowDistance;
        
        public void LoadDefaults()
        {
            Speed = 80;
            RotationSpeed = 1000;
            FollowDistance = 10;
        }
    }
}