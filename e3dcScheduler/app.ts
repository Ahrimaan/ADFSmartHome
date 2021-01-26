import * as cron from 'cron'

const job = new cron.CronJob('*/1 * * * * *', () => {
    console.log('This will get executed every second');
});

job.start();