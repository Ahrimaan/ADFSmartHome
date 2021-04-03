import schedule
import time
import os
from api import get_pv_values
import pika
import json

rabbit_server = os.getenv('RABBIT_SERVER')
rabbit_user=os.getenv('RABBIT_USER')
rabbit_password=os.getenv('RABBIT_PASS')
rabbit_port=os.getenv('RABBIT_PORT')
rabbit_exchange=os.getenv('RABBIT_EXCHANGE')
rabbit_routingkey=os.getenv('RABBIT_ROUTINGKEY')


connection = pika.BlockingConnection(pika.ConnectionParameters(host=rabbit_server,port=rabbit_port,credentials=pika.PlainCredentials(rabbit_user,rabbit_password)))
channel = connection.channel()

def read_pv_data():
    pv_data = get_pv_values()
    pv_data["id"] = "e3dc_adf"
    send_data_to_queue(pv_data)

def send_data_to_queue(data):
    channel.basic_publish(rabbit_exchange,rabbit_routingkey,json.dumps(data))

def main():
    while True:
        schedule.run_pending()
        time.sleep(0.2)

schedule.every(5).seconds.do(read_pv_data)

if __name__ == "__main__":
    main()
