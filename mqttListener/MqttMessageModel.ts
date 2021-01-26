export interface MqttMessage {
    Time: Date,
    id:string,
    ENERGY: {
        TotalStartTime: Date
        Total: Number,
        Yesterday: Number,
        Today: Number,
        Period: Number,
        Power: Number,
        ApparentPower: Number,
        ReactivePower: Number,
        Factor: Number,
        Voltage: Number,
        Current: Number
    }
}