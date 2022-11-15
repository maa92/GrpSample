Highcharts.chart('dSysDBHajjReq', {
    chart: {
        type: 'column'
    },
    title: {
        text: 'طلبات الترشيح لموسم الحج',
        useHTML: Highcharts.hasBidiBug
    },

    data: {
        rowsURL: 'http://localhost:5871/App/TestChart',// window.location.href + 'App/TestChart',
        //rows:[["1431","1500"],["1432","1700"],["1433","1400"],["1434","1200"],["1435","1800"],["1436","2000"],["1437","1700"],["1438","1700"],["1439","1750"]],
        firstRowAsNames: false//,
        //enablePolling: true
    },

    /*xAxis: {
        tickInterval: 7 * 24 * 3600 * 1000, // one week
        tickWidth: 0,
        gridLineWidth: 1,
        labels: {
            align: 'left',
            x: 3,
            y: -3
        }
    },*/

    credits: {
        enabled: false
    }
});

Highcharts.chart('dSysDBHajjReq2', {
    chart: {
        type: 'line'
    },
    title: {
        text: 'طلبات الترشيح لموسم الحج',
        useHTML: Highcharts.hasBidiBug
    },

    data: {
        rowsURL: 'http://localhost:5871/App/TestChart',// window.location.href + 'App/TestChart',
        //rows:[["1431","1500"],["1432","1700"],["1433","1400"],["1434","1200"],["1435","1800"],["1436","2000"],["1437","1700"],["1438","1700"],["1439","1750"]],
        firstRowAsNames: false//,
        //enablePolling: true
    },
    credits: {
        enabled: false
    }
});

Highcharts.chart('dSysDBHajjReq3', {
    chart: {
        type: 'pie'
    },
    title: {
        text: 'طلبات الترشيح لموسم الحج',
        useHTML: Highcharts.hasBidiBug
    },

    data: {
        rowsURL: 'http://localhost:5871/App/TestChart',// window.location.href + 'App/TestChart',
        //rows:[["1431","1500"],["1432","1700"],["1433","1400"],["1434","1200"],["1435","1800"],["1436","2000"],["1437","1700"],["1438","1700"],["1439","1750"]],
        firstRowAsNames: false//,
        //enablePolling: true
    },
    credits: {
        enabled: false
    }
});

    /*var chart = new Highcharts.Chart({

        chart: {
            renderTo: 'dSysDBHajjReq',
            type: 'column'
        },

        xAxis: {
            categories: [
                '1432',
                '',
                '',
                '',
                '',
                ''
            ],
            //reversed: true
        },

        yAxis: {
            title: {
                text: 'الأسماء الشعبية',
                useHTML: Highcharts.hasBidiBug
            },
            opposite: true
        },

        title: {
            text: 'بعض الأسماء الشعبية',
            useHTML: Highcharts.hasBidiBug
        },

        subtitle: {
            text: 'Data input from a remote JSON file'
        },

        legend: {
            useHTML: Highcharts.hasBidiBug
        },

        tooltip: {
            useHTML: true
        },

        series: [{
            name: 'اسماء الذكور',
            data: [1, 3, 2, 4, 3, 5]
        },
        {
            //name: 'Installation',
            type: 'line',
            data: [1, 3, 2, 4, 3, 5]
        }
        ]

    });*/
//});