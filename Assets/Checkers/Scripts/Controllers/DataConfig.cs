using UnityEngine;

namespace Checkers
{
    public class DataConfig : Singleton<DataConfig>
    {
        [Tooltip("Colors: ")]
        public Color32 ChoosenCheckerColor = new Color32(0, 122, 255, 255);
        public Color32 AvailableForMoveCheckerColor = new Color32(242, 136, 23, 255);

        [Space]
        public float UndoAnimationTime = 0.2f;

        [Tooltip("How many time game wait after click on NoAdsButton and showing rewarded video.")]
        public float NoAdsRewardAppearDelay = 3f;
        [Tooltip("Delay before AI move.")]
        public float AIMoveTime = 1f;
    }
}