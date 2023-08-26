using System;

namespace _Scripts.Counters
{
    public interface IHasProgress
    {
        public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;

        public class OnProgressChangedEventArgs : EventArgs
        {
            public float ProgressNormalized;
        }
    }
}