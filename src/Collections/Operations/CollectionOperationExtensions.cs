namespace SharperExtensions.Collections;

public static class CollectionOperationExtensions
{
    extension<T>(OptionSequence<T> optionSequence)
        where T : notnull
    {
        public Option<T> TakeFirstSome() =>
            OptionSequenceOperations.TakeFirstSome(optionSequence);
        
        public Option<T> TakeLastSome() =>
            OptionSequenceOperations.TakeLastSome(optionSequence);

    }
}