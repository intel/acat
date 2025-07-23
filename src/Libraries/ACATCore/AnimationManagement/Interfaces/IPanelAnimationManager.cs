using ACAT.Core.Interpreter;
using ACAT.Core.PanelManagement;
using ACAT.Core.WidgetManagement;
using System.Collections.Generic;

namespace ACAT.Core.AnimationManagement
{
    public interface IPanelAnimationManager : IAnimationManager
    {
        event PanelAnimationManager.PlayerStateChanged EvtPlayerStateChanged;
        event PanelAnimationManager.ResolveWidgetChildren EvtResolveWidgetChildren;

        bool Init(PanelConfigMapEntry panelConfigMapEntry, Widget panelWidget = null);
        void Start(Widget panelWidget, string animationName = null);
    }
}