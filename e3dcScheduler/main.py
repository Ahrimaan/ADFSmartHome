import schedule
import time
from api import get_values


def getApiValuesJob():
    result = get_values()
    result["id"] = "e3dc_adf"
    # Send Data to Queue
    print(result)

def main():
    while True:
        schedule.run_pending()
        time.sleep(0.2)

schedule.every(5).seconds.do(getApiValuesJob)

if __name__ == "__main__":
    main()
