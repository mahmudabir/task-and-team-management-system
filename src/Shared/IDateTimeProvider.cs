﻿namespace Shared;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
