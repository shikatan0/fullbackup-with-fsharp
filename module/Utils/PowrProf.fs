module PowrProf

[<System.Runtime.InteropServices.DllImport("PowrProf.dll", SetLastError = true)>]
extern bool SetSuspendState(bool bHibernate, bool bForce, bool bWakeupEventsDisabled)