export interface MqttMessage {
    Time: Date,
    id:string,
    ENERGY: {
        Yesterday: Number,
        Today: Number,
        Power: Number,
        Total: Number,
        Voltage:Number
        Current:Number
    }
}