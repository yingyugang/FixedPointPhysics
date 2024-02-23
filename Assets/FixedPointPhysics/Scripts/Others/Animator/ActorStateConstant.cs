namespace BlueNoah.PhysicsEngine
{
    /// <summary>
    /// Defines constants for actor states used in animations. These constants are used to control
    /// and identify different states of an actor within a physics-based game or simulation environment.
    /// </summary>
    public static class ActorStateConstant 
    {
        public const int ANIM_NORMAL = 0; 
        public const int ANIM_FALLING = 2; 
        public const int ANIM_JUMP = 3; 

        public const int ANIM_RUN = 4; 

        public const int ANIM_DIVE = 5; 
        public const int ANIM_DIVE_CONTINUE = 6; 
        public const int ANIM_DIVE_GETUP = 7; 

        public const int ANIM_DOWN = 8; 
        public const int ANIM_DOWN_CONTINUE = 9; 
        public const int ANIM_DOWN_GETUP = 10;

        public const int ANIM_VICTORY = 99; 
        public const int ANIM_SKY_DIVING = 100;
        public const string PARAM_STATE = "State"; 
        public const string PARAM_MOVE = "Move"; 
        public const string PARAM_Heavy = "Heavy"; 
    }
}