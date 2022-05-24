namespace DominoPlayer;

[System.Serializable]
public class DominoException : System.Exception
{
    public DominoException() { }
    public DominoException(string message) : base(message) { }
    public DominoException(string message, System.Exception inner) : base(message, inner) { }
    protected DominoException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}