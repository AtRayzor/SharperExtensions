namespace SharperExtensions;

public interface IOption<out T>
    where T : notnull;
