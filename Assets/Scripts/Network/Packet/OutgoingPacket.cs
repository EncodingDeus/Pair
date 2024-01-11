namespace Dobrozaur.Network.Packet
{
    public class OutgoingPacket<T>
    {
        public OutgoingPacket(bool error, T payload)
        {
            Error = error;
            Payload = payload;
        }

        public bool Error; 
        public T Payload;
    }
}