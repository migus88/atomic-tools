using Atomic.Injector.Generators.Enums;

namespace Atomic.Injector.Generators.Models
{
    public class InstallModel
    {

        public string BoundType { get; set; }
        public bool IsLazy
        {
            get => _isLazy;
            set => _isLazy = value || Mode == InstallMode.Transient;
        }
        public InstallMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                if (_mode == InstallMode.Transient)
                {
                    IsLazy = true;
                }
            }
        }


        private InstallMode _mode = InstallMode.Transient;
        private bool _isLazy = false;
    }
}