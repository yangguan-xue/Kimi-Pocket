namespace KimiPocket.Core.Interaction;

public enum SuppressionReason { Menu, Settings, SessionLock }
public enum HoverState { Hidden, PendingOpen, Visible, PendingClose, Suppressed }
// Time is monotonic milliseconds supplied by the host; no delayed callbacks can revive old states.
public sealed class HoverCoordinator
{
    public HoverState State { get; private set; }
    public bool IsVisible => State is HoverState.Visible or HoverState.PendingClose;
    public bool NeedsProbe => State is HoverState.PendingOpen or HoverState.Visible or HoverState.PendingClose;
    public bool Keyboard { get; private set; }
    private readonly HashSet<SuppressionReason> suppressions = [];
    private long deadline;
    private bool disarmed;
    public void Move(bool icon, bool panel, long now)
    {
        if (!icon) disarmed = false;
        if (State == HoverState.Suppressed) return;
        if (Keyboard) { if (panel) Keyboard = false; else return; }
        if (icon || panel)
        {
            if (State == HoverState.PendingClose) State = HoverState.Visible;
            if (icon && !disarmed && State == HoverState.Hidden) { State = HoverState.PendingOpen; deadline = now + 180; }
        }
        else if (State == HoverState.PendingOpen) State = HoverState.Hidden;
        else if (State == HoverState.Visible) { State = HoverState.PendingClose; deadline = now + 300; }
    }
    public void Tick(long now) { if (now < deadline) return; if (State == HoverState.PendingOpen) State = HoverState.Visible; else if (State == HoverState.PendingClose) State = HoverState.Hidden; }
    public void Toggle(long now) { if (State == HoverState.Suppressed) return; if (IsVisible) { Hide(); } else { Keyboard = false; State = HoverState.Visible; } }
    public void OpenKeyboard() { if (State == HoverState.Suppressed) return; Keyboard = true; State = HoverState.Visible; }
    public void Hide() { Keyboard = false; State = suppressions.Count > 0 ? HoverState.Suppressed : HoverState.Hidden; disarmed = true; }
    public void Suppress(SuppressionReason reason = SuppressionReason.Menu) { suppressions.Add(reason); Hide(); }
    public void Release(SuppressionReason reason = SuppressionReason.Menu) { suppressions.Remove(reason); Hide(); }
}
