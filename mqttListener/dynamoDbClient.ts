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
            id:item.id,
            date:item.Time,
            energy:item.ENERGY
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