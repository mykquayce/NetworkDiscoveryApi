namespace System;

public static class SystemExtensions
{
	public static TimeSpan Clamp(this TimeSpan value, TimeSpan min, TimeSpan max)
		=> TimeSpan.FromTicks(long.Clamp(value.Ticks, min.Ticks, max.Ticks));
}
