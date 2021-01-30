import schedule
import time
import datetime
import os

from api import get_values
from influx import saveData


def job():
    result = get_values()
    result["id"] = "e3dc_adf"
    saveData(result)

schedule.every(5).seconds.do(job)

while True:
    schedule.run_pending()
    time.sleep(0.2)
