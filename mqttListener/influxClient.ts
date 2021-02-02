import { InfluxDB, Point, HttpError } from '@influxdata/influxdb-client'
import { env } from 'process'
import { MqttMessage } from './MqttMessageModel'

const influxClient = new InfluxDB({ url: env.INFLUX_URL })
console.log(env.INFLUX_URL)
export const saveData = (item: MqttMessage) => {
    let dataPoint = new Point('smarthome');
    dataPoint.stringField('TotalStartTime', item.ENERGY.TotalStartTime)
        .stringField('ID',item.id.substr(item.id.indexOf('/') + 1, item.id.lastIndexOf('/')   - item.id.indexOf('/') -1 ))
        .floatField('Total', item.ENERGY.Total)
        .floatField('Yesterday', item.ENERGY.Yesterday)
        .floatField('Today', item.ENERGY.Today)
        .floatField('Power', item.ENERGY.Power)
        .floatField('ApparentPower', item.ENERGY.ApparentPower)
        .floatField('ReactivePower', item.ENERGY.ReactivePower)
        .floatField('Voltage', item.ENERGY.Voltage)
        .floatField('Current', item.ENERGY.Current)
        .timestamp(new Date())
    let writeApi = influxClient.getWriteApi('smarthome', 'smarthome')
    writeApi.writePoint(dataPoint)
    writeApi
        .close()
        .then(() => {
            console.log('FINISHED')
        })
        .catch(e => {
            console.error(e)
            if (e instanceof HttpError && e.statusCode === 401) {
                console.log(e)
            }
            console.log('\nFinished ERROR')
        })
}