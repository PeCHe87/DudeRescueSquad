namespace DudeResqueSquad
{
    public static class Enums
    {
        public enum CharacterStates { NONE, IDLE, RUNNING, ATTACKING }

        public enum ItemType { NONE, WEAPON, COLLECTABLE, KEY }

        public enum KeyType { NONE, REGULAR, SPECIAL, SKELETON }

        public enum SwipeType { NONE, UP, DOWN, LEFT, RIGHT }

        public enum WeaponAttackType { NONE, MELEE_2_HANDS, ASSAULT_2_HANDS, MELEE_1_HAND, ASSAULT_1_HAND }
    }
}