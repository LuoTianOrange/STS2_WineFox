namespace STS2_WineFox.Cards.Token.HandCrank
{
    /// <summary>
    ///     Marker interface for token cards that should apply their effect directly
    ///     without entering the hand, similar to <c>KnowledgeDemon.IChoosable</c>.
    ///     Callers should cast the card to <see cref="IDirectApply" /> and invoke
    ///     <see cref="Apply" /> instead of adding the card to any pile.
    /// </summary>
    public interface IDirectApply
    {
        Task Apply();
    }
}

