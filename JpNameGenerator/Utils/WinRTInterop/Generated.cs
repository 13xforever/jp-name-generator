using System;

namespace JpNameGenerator.Utils.WinRTInterop;

internal enum TrustLevel
{
    BaseTrust,
    PartialTrust,
    FullTrust
}

internal unsafe partial interface IInspectable : global::MicroCom.Runtime.IUnknown
{
    void GetIids(ulong* iidCount, Guid** iids);
    IntPtr RuntimeClassName { get; }
    TrustLevel TrustLevel { get; }
}

internal unsafe partial interface IActivationFactory : IInspectable
{
    IntPtr ActivateInstance();
}

internal unsafe partial interface IUISettings3 : IInspectable
{
    WinRTColor GetColorValue(UIColorType desiredColor);
}
