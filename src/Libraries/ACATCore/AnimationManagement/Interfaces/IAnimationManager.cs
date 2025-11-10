using ACAT.Core.Interpreter;
using ACAT.Core.WidgetManagement;
using System.Collections.Generic;

namespace ACAT.Core.AnimationManagement.Interfaces
{
    public interface IAnimationManager
    {
        Interpret Interpreter { get; }
        bool IsSwitchActive { get; set; }

        void Dispose();
        PlayerState GetPlayerState();
        void HighlightDefaultHome();
        //bool Init();
        void Interrupt();
        void Pause();
        List<string> ResolveArgs(List<string> args);
        bool ResolveBool(string arg);
        void Restart();
        void Resume();
        void SetSelectedWidget(string widgetName);
        void SetSelectedWidget(Widget selectedWidget);
        //void Start();
        void Stop();
        void Transition(Animation animation = null);
    }
}