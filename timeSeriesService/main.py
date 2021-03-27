import requests
import pika
import os
import json
import dateutil.parser as parser
import time

from requests.api import head

rabbit_server = os.getenv('RABBIT_SERVER')
rabbit_user=os.getenv('RABBIT_USER')
rabbit_password=os.getenv('RABBIT_PASS')
rabbit_port=os.getenv('RABBIT_PORT')
rabbit_queue=os.getenv('RABBIT_QUEUE')
influx_host=os.getenv('INFLUX_HOST')
influx_port=os.getenv('INFLUX_PORT')
influx_token=os.getenv('INFLUX_TOKEN')
influx_bucket=os.getenv('INFLUX_BUCKET')
influx_org=os.getenv('INFLUX_ORG')
plug_routing_key=os.getenv('ROUTINGKEY_PLUG')
influx_url = 'http://{influxhost}:{influxport}/api/v2/write?bucket={influxbucket}&precision=s&org={influxorg}'
smartplug_datatemplate = "plugs,Device={device} Total={total},Yesterday={yesterday},Today={today},Power={power},Voltage={voltage},Current={current} {time}"

def setup_queue():
    
    connection = pika.BlockingConnection(pika.ConnectionParameters(host=rabbit_server,port=rabbit_port,credentials=pika.PlainCredentials(rabbit_user,rabbit_password)))
    channel = connection.channel()
    channel.basic_consume(rabbit_queue,auto_ack=False,on_message_callback=callback)
    channel.start_consuming()

def callback(ch, method, properties, body):
    print(" [x] Received %r" % body)
    
    data_recieved_bytes = body.decode('utf8').replace("'", '"')
    data_recieved_json = json.loads(data_recieved_bytes)
    if save_timeseries_data(data_recieved_json,method.routing_key):
        ch.basic_ack(delivery_tag=method.delivery_tag)
    else:
        ch.basic_reject(delivery_tag=0, requeue=True)


def main():
    setup_queue()
    

def save_timeseries_data(queue_data, routing_key):
    data = None
    
    if routing_key == plug_routing_key:
        print("Recieved requst to save data for routing key:{}",routing_key)
        data = get_plug_data(queue_data)
    token = "Token {influx_token}".format(influx_token=influx_token)
    headers = {'Authorization': token}
    url = influx_url.format(influxhost=influx_host,influxport=influx_port,influxbucket=influx_bucket,influxorg=influx_org)
    print("Sending data to {}",url)
    try:
        response = requests.post(url,headers=headers,data=data)
        print("Sending data to Influx, recieved code {code} content:{data}".format(code=response.status_code,data=response.content))
        return response.status_code == 204
    except requests.exceptions.RequestException:
        return False    

def get_plug_data(data):
    device = data["Device"]
    total=data["Total"]
    timestamp= int(parser.parse(data["Time"]).timestamp())
    yesterday= data["Yesterday"]
    today=data["Today"]
    power=data["Power"]
    voltage=data["Voltage"]
    current=data["Current"]
    smartplug_data = smartplug_datatemplate.format(device=device,total=total,yesterday=yesterday,today=today,power=power,voltage=voltage,current=current,time=timestamp)
    print("Formatted Data:{}",smartplug_data)
    return smartplug_data


if __name__ == "__main__":
    main()
