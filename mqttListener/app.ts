import { config } from 'dotenv'
import { connect, IPacket, ISubscriptionGrant, IClientOptions } from 'mqtt';
import { env } from 'process';
config();


class ClientOptions implements IClientOptions {
    connectTimeout = 2;
    keepalive = 2;
    resubscribe = true;
}

let topics = env.TOPICS.split(',');
let server = env.SERVER;
let client = connect(`mqtt://${server}`, new ClientOptions());

console.log(`Try to listening on ${server} to topics ${topics.join(',')}`)

client.on('error', (err: IPacket) => {
    console.log(`Error from server ${server} error message: ${err.cmd} `);
});

client.on('offline', () => {
    throw Error(`There was an error connecting to the Server ${server}`)
})

client.on('connect', (message: IPacket) => {
    console.log(`Connected with server ${server} connectonmessage: ${message.cmd} `);
    client.subscribe(topics, null, (err: Error, grant: ISubscriptionGrant[]) => {
        if (err !== undefined) {
            console.error(`Error in subscription ${err.message}`)
        } else {
            console.log(`Subscribed to ${grant.length} topics`)
        }

    });
});

client.on('message', (topic: string, payload: Buffer, packet: IPacket) => {
    console.log(`Recieved message in topic: ${topic} , payload:${payload.toString()} , packetCommand: ${packet.cmd}`)
});

