namespace smarthome.mqttService.Contracts
{
    public interface IAmqpClient
    {
        public void SendMessageToQueue(string message);
    }
}
