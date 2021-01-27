from pymodbus.client.sync import ModbusTcpClient
import os

ip = os.getenv('E3DC_IP')
print(f'Connecting to {ip}')
client = ModbusTcpClient(ip)
print(f'Current connection state is: {client.state}')
# all register -1 index 0
register_feed = 40068 - 1
register_batteryCurrent = 40070 - 1
register_currentToExternal = 40074 - 1
register_battery_Percent = 40083 - 1
register_house_load = 40072 - 1
register_self_sufficiency = 40082 - 1


def get_feed():
    return str(__read_value(register_feed, 1))


def get_battery_current():
    return str(__read_value(register_batteryCurrent, 1))


def get_self_sufficiency():
    value = __read_value(register_self_sufficiency, 1)
    return [value >> 8, 255 & value]


def get_current_from_external():
    result = __read_value(register_currentToExternal, 2)
    if result[1] > 0:
        value = result[0] - result[1]
        return str(value)
    return str(result[0])


def get_battery_load():
    return str(__read_value(register_battery_Percent, 1))


def get_house_current():
    return str(__read_value(register_house_load, 1))


def __read_value(register, length):
    result = client.read_holding_registers(register, length)
    print("Register:%s Value:%s" % (register, result.registers))
    if length > 1:
        return result.registers
    return result.registers[0]
