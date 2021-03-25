import schedule
import time
import os
from api import get_values
import pika

rabbit_server = os.getenv('RABBIT_SERVER')
rabbit_user=os.getenv('RABBIT_USER')
rabbit_password=os.getenv('RABBIT_PASS')
rabbit_port=os.getenv('RABBIT_PORT')
rabbit_exchange=os.getenv('RABBIT_QUEUE')
rabbit_routingkey=os.getenv('RABBIT_ROUTINGKEY')


connection = pika.BlockingConnection(pika.ConnectionParameters(host=env,port=rabbit_port,credentials=pika.PlainCredentials(rabbit_user,rabbit_password)))
channel = connection.channel()

def getApiValuesJob():
    result = get_values()
    result["id"] = "e3dc_adf"
    # Send Data to Queue
    sendData(result)

def sendData(data):
    channel.basic_publish(rabbit_exchange,rabbit_routingkey,data)

def main():
    while True:
        schedule.run_pending()
        time.sleep(0.2)

schedule.every(5).seconds.do(getApiValuesJob)

if __name__ == "__main__":
    main()
