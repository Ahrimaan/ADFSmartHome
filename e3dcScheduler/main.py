import schedule
import time

from api import get_values


def job():
    print(get_values())

schedule.every(5).seconds.do(job)

while True:
    schedule.run_pending()
    time.sleep(0.2)
