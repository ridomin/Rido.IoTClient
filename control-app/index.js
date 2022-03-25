const gbid = id => document.getElementById(id)
const js = o => JSON.stringify(o, null,2)

//const hivehost = 'f8826e3352314ca98102cfbde8aff20e.s2.eu.hivemq.cloud'
const host = 'f8826e3352314ca98102cfbde8aff20e.s2.eu.hivemq.cloud'
const username = 'client2'
const password = 'Myclientpwd.000'

const clientId = 'webApp' + Date.now()

let onCommandResponse = resp => {}

let client, data, chart, chartOptions, did

google.charts.load('current', { packages: ['corechart', 'line'] })
google.charts.setOnLoadCallback( () => {
    data = new google.visualization.DataTable();
    data.addColumn('date','time');
    data.addColumn('number','workingSet');
    chartOptions = { hAxis: { title: 'Time',direction:1},
                     vAxis: { title: 'WorkingSet'},
                    animation:{ duration: 500,easing: 'out'}}
    chart = new google.visualization.LineChart(gbid('chart_div'))
})

gbid('button_set_interval').onclick = () => {
    const interval = parseInt(gbid('input_interval').value, 10)
    client.publish(`pnp/${did}/props/set`, js({interval}))
}

gbid('button_set_enabled').onclick = () => {
    const enabled = String(gbid('input_enabled').value)=='true'
    console.log(enabled)
    client.publish(`pnp/${did}/props/set`, js({enabled}))
}

gbid('button_cmd_getRuntimeStats').onclick = async () => {
    const diagMode = parseInt(gbid('cmd_getRuntimeStats_diagnosticsMode').options.selectedIndex,10)
    const cmdResp = await cmd_getRuntimeStats(diagMode)
    gbid('cmd_getRuntimeStats_response').innerText = js(cmdResp)
}

const cmd_getRuntimeStats = diagMode => {
    client.publish(`pnp/${did}/commands/getRuntimeStats`, js(diagMode))
    return new Promise((resolve,reject) => {
        onCommandResponse = resp => resolve(resp)
    })
}

const updateReported = twin => {
    const tileInterval = gbid('reported-interval').children.item(1)
    
    if (twin.started) {
        const tileStarted = gbid('reported-started').children.item(1)
        const startedValue = new Date(twin.started).toLocaleString()
        tileStarted.innerText = startedValue
    }
    if (twin.enabled) {
        const tileEnabled = gbid('reported-enabled')
        tileEnabled.children.item(1).innerText = twin.enabled.value
        tileEnabled.children.item(2).innerText = 'ac: ' + twin.enabled.ac
        tileEnabled.children.item(3).innerText = new Date().toLocaleString()
    }
    if (twin.interval) {
        const tileInterval = gbid('reported-interval')
        tileInterval.children.item(1).innerText = twin.interval.value
        tileInterval.children.item(2).innerText = 'ac: ' + twin.interval.ac
        tileInterval.children.item(3).innerText = new Date().toLocaleString()
    }
}


;(async () => {
    const options = {clientId, username, password}
    client = mqtt.connect(`wss://${host}:8884/mqtt`, options)
    client.on('connect', () => {
        console.log('connected')
        
        client.subscribe('pnp/+/birth', e => {
            if (e) throw e
            console.log('subscribed to birth')
        })

        client.subscribe('pnp/+/telemetry', e => {
            if (e) throw e
            console.log('subscribed to telemetry')
        })

        client.subscribe('pnp/+/props/reported/#', e => {
            if (e) throw e
            console.log('susbscribed to props reported')
        })

        client.subscribe('pnp/+/commands/+/resp/#', e => {
            if (e) throw e
            console.log('susbscribed to command reponses')
        })

        client.on('message', (t, m) => {
            const msg = m ? JSON.parse(m.toString()) : {};
            //console.log(t, msg)
            if (t.indexOf(`birth`)>0) {
                did = t.split('/')[1]
                console.log('BIRTH:', did)
            }

            if (t.startsWith(`pnp/${did}/telemetry`)) {
                data.addRow([new Date(),msg.workingSet])
                chart.draw(data, chartOptions);
            }

            if (t.startsWith(`pnp/${did}/commands`)) {
                console.log('command accepted: ', t)
                onCommandResponse(msg)
            }

            if (t.startsWith('pnp/${did}/props/reported'))
            {
                console.log(msg)
                updateReported(msg)
                if (msg.interval) gbid('input_interval').value = msg.interval.value
                if (msg.enabled) gbid('input_enabled').value = msg.enabled.value
            }

        })
    })
})()