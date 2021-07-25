namespace DudeRescueSquad.Core.Characters
{
    public interface ICharacterAbility
    {
        bool WasInitialized();
        bool IsEnabled();
        void EarlyProcessAbility();
        void Process();
        void Initialization();
    }
}