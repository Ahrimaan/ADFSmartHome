import { Credentials, DynamoDB } from 'aws-sdk'
import { env } from 'process';
import { MqttMessage } from './MqttMessageModel';

const dynamoDb = new DynamoDB.DocumentClient(
    {
        region: env.AWS_REGION,
        credentials: new Credentials(env.AWS_CLIENTID,env.AWS_CLIENTSECRET)
    }
);

export async function createItem(item: MqttMessage): Promise<boolean> {
    const params = {
        TableName: process.env.DYNAMODB_TABLE,
        Item: {
            id: item.id.substr(item.id.indexOf('/') + 1, item.id.lastIndexOf('/')   - item.id.indexOf('/') -1 ),
            date:item.Time,
            startDate:item.ENERGY.TotalStartTime,
            totalConsumption:item.ENERGY.Total,
            currentPowerDraw:item.ENERGY.Power,
            currentApparentPower: item.ENERGY.ApparentPower,
            currentReactivePower: item.ENERGY.ReactivePower,
            voltage:item.ENERGY.Voltage,
            current:item.ENERGY.Current
        }
    }
    try {
        let result = await dynamoDb.put(params).promise();
        return result.$response.httpResponse.statusCode === 200;
    } catch (error: any) {
        console.log(error);
        return false;
    }
} 