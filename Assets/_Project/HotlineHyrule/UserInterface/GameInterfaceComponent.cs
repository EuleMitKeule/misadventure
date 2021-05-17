using UnityEngine;

namespace HotlineHyrule.UserInterface
{
    public class GameInterfaceComponent : MonoBehaviour
    {
        [SerializeField] public GameObject levelInfoParent;
        
        Animator LevelInfoAnimator { get; set; }

        void Awake()
        {
            if (!levelInfoParent) levelInfoParent = transform.Find("parent_level_info").gameObject;
            if (levelInfoParent) LevelInfoAnimator = levelInfoParent.GetComponent<Animator>();
        }
        
        public void ShowLevelInfo()
        {
            if (LevelInfoAnimator) LevelInfoAnimator.SetTrigger("show");
        }
        
        public void HideLevelInfo()
        {
            if (LevelInfoAnimator) LevelInfoAnimator.SetTrigger("hide");
        }
    }
}