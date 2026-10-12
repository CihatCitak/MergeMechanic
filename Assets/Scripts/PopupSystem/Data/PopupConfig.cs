using DG.Tweening;
using UnityEngine;

namespace Game.PopupSystem
{
    [CreateAssetMenu(menuName = "Game/Popup Config", fileName = "PopupConfig")]
    internal class PopupConfig : ScriptableObject
    {
        [SerializeField, Min(0f)] private float _openDuration = 0.25f;
        [SerializeField, Min(0f)] private float _closeDuration = 0.15f;
        [SerializeField, Range(0f, 1f)] private float _startScale = 0.8f;
        [SerializeField] private Ease _openEase = Ease.OutBack;
        [SerializeField, Range(0f, 1f)] private float _dimAlpha = 0.5f;

        public float OpenDuration => _openDuration;
        public float CloseDuration => _closeDuration;
        public float StartScale => _startScale;
        public Ease OpenEase => _openEase;
        public float DimAlpha => _dimAlpha;
    }
}
