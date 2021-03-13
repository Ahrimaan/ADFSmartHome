from pymodbus.client.sync import ModbusTcpClient
import os
from datetime import datetime

ip = os.getenv('E3DC_IP')
print(f'Connecting to {ip}')
client = ModbusTcpClient('192.168.178.75')
# all register -1 index 0
register_feed = 40068 - 1
register_batteryCurrent = 40070 - 1
register_currentToExternal = 40074 - 1
register_battery_Percent = 40083 - 1
register_house_load = 40072 - 1
register_self_sufficiency = 40082 - 1
register_voltage_1 = 40096 - 1
register_voltage_2 = 40097 - 1
register_current_1 = 40099 - 1
register_current_2 = 40100 - 1
register_power_1 = 40102 - 1
register_power_2 = 40103 - 1


def get_values():
    values = __read_value(40067, 16)
    tracker_data = __read_value(40095, 9)
    data = {
        "date": f'{datetime.now().date()}  {datetime.now().time()}',
        "pvCurrent": values[0],
        "battery": __read_battery_current(values),
        "consumption": values[4] + values[5],
        "external": __read_external_current(values),
        "batteryLoad": values[15],
        "tracker_voltage_1": tracker_data[0],
        "tracker_voltage_2": tracker_data[1],
        "tracker_current_1": tracker_data[3],
        "tracker_current_2": tracker_data[4],
        "tracker_power_1": tracker_data[6],
        "tracker_power_2": tracker_data[7],
    }
    return data


def __read_battery_current(values):
    if values[3] > 0:
        return values[2] - values[3]
    return values[2]


def __read_external_current(values):
    if values[7] > 0:
        return values[6] - values[7]
    return values[6]


def __read_self_sufficiency():
    value = __read_value(register_self_sufficiency, 1)
    return [value >> 8, 255 & value]


def __read_value(register, length):
    result = client.read_holding_registers(register, length)
    print("Register:%s Value:%s" % (register + 1, result.registers))
    if length > 1:
        return result.registers
    return result.registers[0]
