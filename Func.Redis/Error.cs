﻿using System.Diagnostics.Contracts;

namespace Func.Redis;

public readonly struct Error
{
    public string Message { get; }
    
    private Error(string message) => Message = message;
    [Pure]
    public static Error New(string message) => new(message);
    [Pure]
    public static Error New(Exception ex) => new(ex.Message ?? string.Empty);
}