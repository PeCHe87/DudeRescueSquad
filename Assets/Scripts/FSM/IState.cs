namespace DudeResqueSquad
{
    public interface IState
    {
        Enums.EnemyStates State();
        void Tick();
        void OnEnter();
        void OnExit();
    }
}