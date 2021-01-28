import { connect, IPacket, ISubscriptionGrant, IClientOptions } from 'mqtt';
import { env } from 'process';
import { ClientOptions } from './ClientOptions';
import { createItem } from './dynamoDbClient';
import { MqttMessage } from './MqttMessageModel'


let topics = env.TOPICS.indexOf(',') > 0 ? env.TOPICS.split(',') : env.TOPICS ;
let server = env.SERVER;
let port = Number.parseInt(env.PORT)
let client = connect(`mqtt://${server}:${port}`, new ClientOptions());

console.log(`Try to listening on ${server}`)

client.on('error', (err: IPacket) => {
    console.log(`Error from server ${server} error message: ${err.cmd} `);
});

client.on('offline', () => {
    throw Error(`There was an error connecting to the Server ${server}`)
})

client.on('connect', (message: IPacket) => {
    console.log(`Connected with server ${server} connectonmessage: ${message.cmd} `);
    client.subscribe(topics, null, (err: Error, grant: ISubscriptionGrant[]) => {
        if (err !== null) {
            console.error(`Error in subscription ${err.message}`)
        } else {
            console.log(`Subscribed to ${Array.isArray(topics) ? topics.join(',') : topics}`)
        }
    });
});

client.on('message',async (topic: string, payload: Buffer, packet: IPacket) => {
    let response : MqttMessage = JSON.parse(payload.toString());
    response.id = topic;
    console.log(`Recieved message in topic: ${topic} , payload:${JSON.stringify(response)} , packetCommand: ${packet.cmd}`)
    if(await createItem(response)) {
        console.log("Item saved to Database");
    }else {
        console.log("Could not save item to Database");
    }    
});
