using DG.Tweening;
using UnityEngine;

namespace Game.ScoreSystem
{
    [CreateAssetMenu(menuName = "Game/Score Config", fileName = "ScoreConfig")]
    internal class ScoreConfig : ScriptableObject
    {
        [SerializeField, Min(0f)] private float _flightDuration = 0.6f;
        [SerializeField] private Ease _flightEase = Ease.InQuad;
        [SerializeField, Min(0f)] private float _iconPunch = 0.25f;
        [SerializeField, Min(0f)] private float _iconPunchDuration = 0.2f;

        public float FlightDuration => _flightDuration;
        public Ease FlightEase => _flightEase;
        public float IconPunch => _iconPunch;
        public float IconPunchDuration => _iconPunchDuration;
    }
}
