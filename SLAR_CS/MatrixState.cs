namespace SLAR_CS
{
    internal class MatrixState : IState
    {
        // енумерація для станів матриці
        public enum State
        {
            Success,
            InvalidInput
        }

        private State currentState;

        public MatrixState()
        {
        }

        // встановлення поточного стану
        public void SetState(object state)
        {
            currentState = (State)state;
        }

        // отримання поточного стану
        public object GetState()
        {
            return currentState;
        }
    }
}