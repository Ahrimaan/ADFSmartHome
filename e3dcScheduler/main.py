import schedule
import time
import boto3
import os

from api import get_values

dynamodb = boto3.client('dynamodb', 
    region_name=os.getenv('REGION_NAME'),
    aws_access_key_id=os.getenv('AWS_KEY'),
    aws_secret_access_key=os.getenv('AWS_SECRET'))


tablename = os.getenv('AWS_TABLENAME')

def saveData(data):
    item = {
        'id': {
            'S':data['id']
        },
        'date': {
            'S': data['date']
        },
        'current': {
            'N':str(data['pvCurrent'])
        },
        'battery': {
            'N':str(data['battery'])
        },
        'consumption': {
            'N':str(data['consumption'])
        },
        'external': {
            'N': str(data['external'])
        },
        'batteryLoad': {
            'N':str(data['batteryLoad'])
        },
        'tracker_voltage_1': {
            'N':str(data['tracker_voltage_1'])
        },
        'tracker_voltage_2': {
            'N':str(data['tracker_voltage_2'])
        },
        'tracker_current_1': {
            'N':str(data['tracker_current_1'])
        },
        'tracker_current_2': {
            'N':str(data['tracker_current_2'])
        },
        'tracker_power_1': {
            'N':str(data['tracker_power_1'])
        },
        'tracker_power_2': {
            'N':str(data['tracker_power_2'])
        }
    }
    dynamodb.put_item(TableName=tablename,Item=item)


def job():
    result = get_values()
    result["id"] = "e3dc_adf"
    print(result)
    saveData(result)

schedule.every(5).seconds.do(job)

while True:
    schedule.run_pending()
    time.sleep(0.2)
