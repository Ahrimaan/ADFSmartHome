import { IClientOptions } from 'mqtt';
import { env } from 'process';

export class ClientOptions implements IClientOptions {
    connectTimeout = Number.parseInt(env.CONNECTION_TIMEOUT);
    keepalive = Number.parseInt(env.KEEP_ALIVE);
    resubscribe = JSON.parse(env.RESUBSCRIBE);
}