namespace DudeRescueSquad.Core.Characters
{
    public interface ICharacterAbility
    {
        bool WasInitialized();
        bool IsEnabled();
        void Process();
        void Initialization();
    }
}