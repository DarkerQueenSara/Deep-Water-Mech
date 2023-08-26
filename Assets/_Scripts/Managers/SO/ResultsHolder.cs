using UnityEngine;

namespace _Scripts.Managers.SO
{
    [CreateAssetMenu(menuName = "Results Holder", order = 1)]
    public class ResultsHolder : ScriptableObject
    {
        [HideInInspector] public bool won;
        [HideInInspector] public int levelReached;
        [HideInInspector] public float timeLeft;
    }
}