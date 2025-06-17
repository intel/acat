using ACAT.Lib.Core.AnimationManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.UserControlManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;

namespace UserControls
{
    [Descriptor("DC7BDC80-60F5-4115-B188-344EFC145DAB",
        "MouseInterfaceUserControl",
        "User Control for Mouse Interface")]
    public partial class MouseInterfaceUserControl : UserControl, IUserControl
    {
        private UserControlKeyboardCommon _keyboardCommon;

        public MouseInterfaceUserControl()
        {
            InitializeComponent();
        }

        public IDescriptor Descriptor => DescriptorAttribute.GetDescriptor(GetType());

        public SyncLock SyncObj => _keyboardCommon.SyncObj;

        public IUserControlCommon UserControlCommon => _keyboardCommon;

        public event AnimationPlayerStateChanged EvtPlayerStateChanged;

        public bool Initialize(UserControlConfigMapEntry mapEntry, TextController textController, IScannerPanel scanner)
        {
            _keyboardCommon = new UserControlKeyboardCommon(this, mapEntry, textController, scanner);

            bool retVal = _keyboardCommon.Initialize();
            _keyboardCommon.AnimationManager.EvtPlayerStateChanged += AnimationManager_EvtPlayerStateChanged;
            return retVal;
        }

        private void AnimationManager_EvtPlayerStateChanged(object sender, PlayerStateChangedEventArgs e)
        {
            EvtPlayerStateChanged?.Invoke(this, e);
        }

        public void OnLoad()
        {
            throw new System.NotImplementedException();
        }

        public void OnPause()
        {
            throw new System.NotImplementedException();
        }

        public void OnResume()
        {
            throw new System.NotImplementedException();
        }

        public void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            throw new System.NotImplementedException();
        }
    }
}
