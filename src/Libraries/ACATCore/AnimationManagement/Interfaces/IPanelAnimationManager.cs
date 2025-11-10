using ACAT.Core.PanelManagement.PanelConfig;
using ACAT.Core.WidgetManagement;

namespace ACAT.Core.AnimationManagement.Interfaces
{
    public interface IPanelAnimationManager : IAnimationManager
    {
        event AnimationManager.PlayerStateChanged EvtPlayerStateChanged;
        event AnimationManager.ResolveWidgetChildren EvtResolveWidgetChildren;

        bool Init(PanelConfigMapEntry panelConfigMapEntry, Widget panelWidget = null);
        void Start(Widget panelWidget, string animationName = null);
    }
}