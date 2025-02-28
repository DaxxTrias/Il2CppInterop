using System;
using System.Runtime.InteropServices;
using Il2CppInterop.Common;
using Microsoft.Extensions.Logging;

namespace Il2CppInterop.Runtime.Injection
{
    internal abstract class Hook<T> where T : Delegate
    {
        private bool _isApplied;
        private T _detour;
        private T _method;
        private T _original;

        public T Original => _original;

        public abstract string TargetMethodName { get; }
        public abstract T GetDetour();
        public abstract IntPtr FindTargetMethod();

        public virtual void TargetMethodNotFound()
        {
            throw new Exception($"Required target method {TargetMethodName} not found");
        }

        public void ApplyHook()
        {
            //Logger.Instance.LogTrace("Start of ApplyHook");
            if (_isApplied)
            {
                //Logger.Instance.LogTrace("Hook already applied");
                return;
            }

            var methodPtr = FindTargetMethod();

            if (methodPtr == IntPtr.Zero)
            {
                //Logger.Instance.LogError("Target method not found");
                TargetMethodNotFound();
                return;
            }

            Logger.Instance.LogTrace("{MethodName} found: 0x{MethodPtr}", TargetMethodName, methodPtr.ToInt64().ToString("X2"));

            _detour = GetDetour();
            Detour.Apply(methodPtr, _detour, out _original);
            _method = Marshal.GetDelegateForFunctionPointer<T>(methodPtr);
            _isApplied = true;
            //Logger.Instance.LogTrace("End of ApplyHook");
        }
    }
}
