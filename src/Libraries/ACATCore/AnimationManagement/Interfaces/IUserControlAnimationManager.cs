using ACAT.Core.UserControlManagement;
using ACAT.Core.WidgetManagement;

namespace ACAT.Core.AnimationManagement.Interfaces
{
    public interface IUserControlAnimationManager : IAnimationManager
    {
        event UserControlAnimationManager.PlayerAnimationTransition EvtPlayerAnimationTransition;
        event AnimationManager.PlayerStateChanged EvtPlayerStateChanged;
        event AnimationManager.ResolveWidgetChildren EvtResolveWidgetChildren;
        bool Init(UserControlConfigMapEntry mapEntry);
        bool IsPlayerRunning();
        void OnLoad(Widget panelWidget, string animationName = null);
        void Start(string animationName = null);
    }
}