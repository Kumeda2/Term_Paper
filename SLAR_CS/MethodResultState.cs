namespace SLAR_CS
{
    internal class MethodResultState: IState
    {
        public MethodResultState() { }

        public enum State
        {
            Success,
            Undefined,
            Inf
        }

        private State currentState;

        public void SetState(object state)
        {
            currentState = (State)state;
        }

        public object GetState()
        {
            return currentState;
        }
    }
}
