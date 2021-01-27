import schedule
import time

from api import get_battery_current, get_current_from_external,\
    get_battery_load, get_house_current, get_feed, get_self_sufficiency


def job():
    self_sufficiency = get_self_sufficiency()
    print(f'Battery: {get_battery_current()} BatteryLoad: {get_battery_load()}'
          f' Feed:{get_feed()} HouseCurrent: {get_house_current()} CurrentExternal: {get_current_from_external()} '
          f'Self sufficiency %: {self_sufficiency[0]} self power consumption %: {self_sufficiency[1]}')


schedule.every(5).seconds.do(job)

while True:
    schedule.run_pending()
    time.sleep(0.2)
