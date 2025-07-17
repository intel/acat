using ACAT.Core.Interpreter;
using ACAT.Core.UserControlManagement;
using ACAT.Core.WidgetManagement;
using System.Collections.Generic;

namespace ACAT.Core.AnimationManagement
{
    public interface IUserControlAnimationManager : IAnimationManager
    {
        event UserControlAnimationManager.PlayerAnimationTransition EvtPlayerAnimationTransition;
        event UserControlAnimationManager.PlayerStateChanged EvtPlayerStateChanged;
        event UserControlAnimationManager.ResolveWidgetChildren EvtResolveWidgetChildren;
        bool Init(UserControlConfigMapEntry mapEntry);
        bool IsPlayerRunning();
        void OnLoad(Widget panelWidget, string animationName = null);
        void Start(string animationName = null);
    }
}