namespace SLAR_CS
{
    internal interface IState
    {
        void SetState(object state);
        object GetState();
    }
}
