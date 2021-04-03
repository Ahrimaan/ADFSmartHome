import { connect as mqttConnect, ISubscriptionGrant } from 'async-mqtt'
import { connect as amqpConnect, Connection, Channel, Replies  } from 'amqplib';
import config from './config';
import mapFile from './mapping.json'

const mqttClient = mqttConnect(config.MqttServerUrl);
const amqpClient = amqpConnect(config.AmqpServerUrl ?? '')
let amqpChannel:Channel

amqpClient.then((connection:Connection) => {
    connection.createChannel().then((chan:Channel) => {
        chan.assertExchange(config.AmqpExchange ?? '','fanout', { durable:true }).then((reply:Replies.AssertExchange) => {
            console.log(`Connected to AMQP Exchange : ${ reply.exchange }`)
            amqpChannel = chan;
        })
    })
}).catch(err => {
    console.error(err);
    process.exit(1);
});

mqttClient.on('connect', () => {
    let topics = config.MqttTopics;
    if (topics) {
        mqttClient.subscribe(topics)
            .then((subGrants: ISubscriptionGrant[]) => {
                subGrants.forEach(subGrant => {
                    console.info(`granted subscription to topic: ${subGrant.topic} with qos ${subGrant.qos}`);
                });
            })
    } else {
        console.error('No Topics definied, quitting now')
        process.exit(2);
    }

});

mqttClient.on('message', (topic, response) => {
    let id = mapFile.mappings.find(x => x.key === topic) ? mapFile.mappings.find(x => x.key === topic)?.value : topic;
    let message = JSON.parse(response.toString());
    console.log(message)
});

mqttClient.on('error', (err) => {
    console.error(err);
    process.exit(-1)
});
