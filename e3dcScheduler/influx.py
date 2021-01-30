from influxdb import InfluxDBClient
import os

databaseName = os.getenv('INFLUX_DB')
client = InfluxDBClient(host=os.getenv('INFLUX_IP'),
                        port=int(os.getenv('INFLUX_PORT')))
dbExist = False
print(f'Ping Result to DB = {client.ping()}')

for db in client.get_list_database():
    if db['name'] == databaseName:
        print(f'Database { databaseName} exists')
        dbExist = True
client.switch_database(databaseName)

if dbExist == False:
    print(f'Creating Database { databaseName }')
    client.create_database(databaseName)


def saveData(item):
    buy = int(item['external']) if int(item['external']) > 0 else 0
    sell = int(item['external']) * -1 if int(item['external']) < 0 else 0
    batDraw = int(item['battery']) if int(item['battery']) > 0 else 0
    batLoad = int(item['battery']) * -1 if int(item['battery']) > 0 else 0
    data = [{
        "measurement": "PV Anlage",
        "time": item["date"],
        "fields": {
            "pvCurrent": int(item['pvCurrent']),
            "batteryDraw": batDraw,
            "batteryLoad": batLoad,
            "consumption": int(item['consumption']),
            "buy": buy,
            "sell": sell,
            "battery": int(item['battery']),
            "tracker_voltage_1": int(item['tracker_voltage_1']),
            "tracker_voltage_2": int(item['tracker_voltage_2']),
            "tracker_current_1": int(item['tracker_current_1']),
            "tracker_current_2": int(item['tracker_current_2']),
            "tracker_power_1": int(item['tracker_power_1']),
            "tracker_power_2": int(item['tracker_power_2']),
        }
    }]
    print(f'Writing to DB: {client.write_points(data)}')
    
