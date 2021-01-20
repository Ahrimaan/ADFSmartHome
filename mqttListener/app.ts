import { config } from 'dotenv'
import { connect,IPacket, ISubscriptionGrant } from 'mqtt';
import { env } from 'process';
config();

let topics = env.TOPICS.split(','); 
let server = env.SERVER;
let client = connect(`mqtt://${server}`);

client.on('error', (err:IPacket) => {
    console.log(`Error from server ${server} error message: ${err.cmd} `);
});
client.on('connect', (message:IPacket) => {
    console.log(`Connected with server ${server} connectonmessage: ${message.cmd} `);
    for (const key in topics) {
        if (Object.prototype.hasOwnProperty.call(topics, key)) {
            const topic = topics[key];
            client.subscribe(topic,null,(err:Error,grant:ISubscriptionGrant[]) => {
                console.log(`Subscription request for: ${topic} result: ${grant[0].topic}`)
            });
        }
    }
});
client.on('message',(topic:string,payload:Buffer,packet:IPacket) => {
    console.log(`Recieved message in topic: ${ topic } , payload:${payload.toString()} , packetCommand: ${packet.cmd}`)
})