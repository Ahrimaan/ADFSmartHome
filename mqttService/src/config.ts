const MqttServerUrl= process.env["MQTT_SERVER_URL"];
const MqttTopics = process.env["MQTT_TOPICS"]?.indexOf(',') &&  process.env["MQTT_TOPICS"].indexOf(',') > 0 ? process.env["MQTT_TOPICS"].split(',') : process.env["MQTT_TOPICS"]
const AmqpServerUrl = process.env["AMQP_SERVER_URL"];
const AmqpExchange = process.env["AMQP_EXCHANGE"];
const AmqpRoutingKey = process.env["AMQP_ROUTINGKEY"];


export default {
    MqttServerUrl,
    MqttTopics,
    AmqpServerUrl,
    AmqpExchange,
    AmqpRoutingKey
}