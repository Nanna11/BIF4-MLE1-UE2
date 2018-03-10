using System;

/// <summary>
/// Eception thrown when a plugin should handle a request it cant handle
/// </summary>
public class ResultDoesNotExistException : Exception
{
    public ResultDoesNotExistException()
    {
    }

    public ResultDoesNotExistException(string message)
        : base(message)
    {
    }

    public ResultDoesNotExistException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public class ResultCannotBeNullException : Exception
{
    public ResultCannotBeNullException()
    {
    }

    public ResultCannotBeNullException(string message)
        : base(message)
    {
    }

    public ResultCannotBeNullException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public class AttributeDoesNotExistException : Exception
{
    public AttributeDoesNotExistException()
    {
    }

    public AttributeDoesNotExistException(string message)
        : base(message)
    {
    }

    public AttributeDoesNotExistException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public class InstanceDoesNotExistException : Exception
{
    public InstanceDoesNotExistException()
    {
    }

    public InstanceDoesNotExistException(string message)
        : base(message)
    {
    }

    public InstanceDoesNotExistException(string message, Exception inner)
        : base(message, inner)
    {
    }
}