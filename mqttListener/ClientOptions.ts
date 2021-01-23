import { IClientOptions } from 'mqtt';

export class ClientOptions implements IClientOptions {
    connectTimeout = 30;
    keepalive = 10;
    resubscribe = true;
}