from pika.adapters.blocking_connection import BlockingChannel, BlockingConnection
import requests
import pika
import os
import json
import dateutil.parser as parser

rabbit_server:str = os.getenv('RABBIT_SERVER')
rabbit_user:str = os.getenv('RABBIT_USER')
rabbit_password:str = os.getenv('RABBIT_PASS')
rabbit_port:str = os.getenv('RABBIT_PORT')
rabbit_queue:str = os.getenv('RABBIT_QUEUE')
influx_host:str = os.getenv('INFLUX_HOST')
influx_port:str = os.getenv('INFLUX_PORT')
influx_token:str = os.getenv('INFLUX_TOKEN')
influx_bucket:str = os.getenv('INFLUX_BUCKET')
influx_org:str = os.getenv('INFLUX_ORG')
plug_routing_key:str = os.getenv('ROUTINGKEY_PLUG')
pv_routing_key:str = os.getenv('ROUTINGKEY_PV')
influx_url:str = 'http://{influxhost}:{influxport}/api/v2/write?bucket={influxbucket}&precision=s&org={influxorg}'
smartplug_datatemplate:str = "plugs,Device={device} Total={total},Yesterday={yesterday},Today={today},Power={power},Voltage={voltage},Current={current} {time}"
pv_template:str = "pv,id={id} current={current},battery={battery},consumption={consumption},external={external},battery_load={battery_load},v_tracker_1={v_tracker_1},v_tracker_2={v_tracker_2},a_tracker_1={a_tracker_1},a_tracker_2={a_tracker_2},w_tracker_1={w_tracker_1},w_tracker_2={w_tracker_2} {date}"

def setup_queue():

    connection:BlockingConnection = pika.BlockingConnection(pika.ConnectionParameters(
        host=rabbit_server, port=rabbit_port, credentials=pika.PlainCredentials(rabbit_user, rabbit_password)))
    channel:BlockingChannel = connection.channel()
    channel.basic_consume(rabbit_queue, auto_ack=False,
                          on_message_callback=on_newmessage_from_rabbit)
    channel.start_consuming()


def on_newmessage_from_rabbit(ch, method, properties, body):
    print(" [x] Received %r" % body)

    data_recieved_bytes:str = body.decode('utf8').replace("'", '"')
    data_recieved_json:any = json.loads(data_recieved_bytes)
    is_successfull = save_timeseries_data(data_recieved_json, method.routing_key)
    if is_successfull:
        ch.basic_ack(delivery_tag=method.delivery_tag)
    else:
        ch.basic_reject(delivery_tag=method.delivery_tag, requeue=True)


def main():
    setup_queue()


def save_timeseries_data(queue_data, routing_key):
    data:dict = None
    
    if routing_key == plug_routing_key:
        print("Recieved requst from plugs")
        data = get_plug_data(queue_data)
    if routing_key == pv_routing_key:
        print("Recieved requst from pv")
        data = get_pv_data(queue_data)
    token = "Token {influx_token}".format(influx_token=influx_token)
    headers = {'Authorization': token}
    url = influx_url.format(influxhost=influx_host, influxport=influx_port,
                            influxbucket=influx_bucket, influxorg=influx_org)
    print("Sending data to {}", url)
    try:
        response = requests.post(url, headers=headers, data=data)
        print("Sending data to Influx, recieved code {code} content:{data}".format(
            code=response.status_code, data=response.content))
        return response.status_code == 204
    except requests.exceptions.RequestException:
        return False


def get_plug_data(data):
    device = data["Device"]
    total = data["Total"]
    timestamp = int(parser.parse(data["Time"]).timestamp())
    yesterday = data["Yesterday"]
    today = data["Today"]
    power = data["Power"]
    voltage = data["Voltage"]
    current = data["Current"]
    smartplug_data = smartplug_datatemplate.format(
        device=device, total=total, yesterday=yesterday, today=today, power=power, voltage=voltage, current=current, time=timestamp)
    print("Formatted Data:{}", smartplug_data)
    return smartplug_data


def get_pv_data(data):
    id = data["id"]
    date = int(parser.parse(data["date"]).timestamp())
    current = data["pvCurrent"]
    battery = data["battery"]
    consumption = data["consumption"]
    external = data["external"]
    battery_load = data["batteryLoad"]
    v_tracker_1 = data["tracker_voltage_1"]
    v_tracker_2 = data["tracker_voltage_2"]
    a_tracker_1 = data["tracker_current_1"]
    a_tracker_2 = data["tracker_current_2"]
    w_tracker_1 = data["tracker_power_1"]
    w_tracker_2 = data["tracker_power_2"]
    pv_data = pv_template.format(current=current, battery=battery, consumption=consumption, external=external, battery_load=battery_load, v_tracker_1=v_tracker_1,
                                            v_tracker_2=v_tracker_2, a_tracker_1=a_tracker_1, a_tracker_2=a_tracker_2, w_tracker_1=w_tracker_1, w_tracker_2=w_tracker_2, id=id,date=date)
    print("Formatted Data: ", pv_data)
    return pv_data


if __name__ == "__main__":
    main()
