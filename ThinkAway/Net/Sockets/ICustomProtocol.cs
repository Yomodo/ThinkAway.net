namespace ThinkAway.Net.Sockets
{
    public interface ICustomProtocol
    {
        object Data { get; }

        ICustomProtocol FromBytes(byte[] data);
        byte[] ToBytes();
    }
}
