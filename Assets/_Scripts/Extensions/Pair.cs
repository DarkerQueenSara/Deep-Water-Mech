using System;

namespace Extensions
{
    [Serializable]
    public class Pair<T,U>
    {
        public T FirstMember;
        public U SecondMember;

        public Pair(T firstMember, U secondMember)
        {
            FirstMember = firstMember; 
            SecondMember = secondMember;
        }
    }
}