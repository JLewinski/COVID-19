var colors = [
    'rgb(0,128,0)',
    'rgb(141,182,0)',
    'rgb(0,255,255)',
    'rgb(253,238,0)',
    'rgb(0,127,255)',
    'rgb(0,0,255)',
    'rgb(0,221,221)',
    'rgb(242,101,34)',
    'rgb(12,35,64)',
    'rgb(150,0,24)',
    'rgb(150,75,0)',
    'rgb(0,71,171)',
    'rgb(255,40,0)',
    'rgb(255,0,79)',
    'rgb(255,223,0)',
    'rgb(0,255,0)',
    'rgb(128,0,0)',
    'rgb(0,250,154)',
    'rgb(255,0,0)',
    'rgb(255,103,0)'
];
var dates;

$(() => {

    function CreateBarPlot(elementId, dates, dataSets, title, type) {
        var ctx = document.getElementById(elementId);
        if (!type) { type = 'bar'; }
        return new Chart(ctx, {
            type: type,
            data: {
                labels: dates,
                datasets: dataSets
            },
            options: {
                legend: {
                    display: false
                },
                scales: {
                    yAxes: [{
                        ticks: {
                            beginAtZero: true
                        }
                    }]
                },
                title: {
                    display: true,
                    text: title
                }
            }
        });
    }

    function UpdateChart(chart, data, title) {
        chart.data.datasets[0].data = data;
        chart.options.title.text = title;
        chart.update();
    }

    function UpdatePercentFatalChart(chartGroup) {
        var offset = 1 * chartGroup.offset;
        var range = 1 * chartGroup.range;
        var percentageData = [];
        var tempDates = [];
        var dataIndex = 0;
        chartGroup.confirmedData.forEach((x, index) => {
            var confirmed = 0;
            var deaths = 0;

            if (range + offset >= chartGroup.deathData.length) {
                return;
            }

            for (var i = 0; i < range; i++) {
                confirmed += chartGroup.confirmedData[index + i];
                deaths += chartGroup.deathData[index + i + offset];
            }
            var tempData = confirmed ? deaths / confirmed * 100 : deaths * 100;

            if (tempData || percentageData.length) {
                percentageData[dataIndex] = tempData;
                tempDates[dataIndex] = dates[index + offset + range / 2];
                if (!tempDates[dataIndex]) {
                    tempDates[dataIndex] = dates[index + offset + range / 2 - 0.5];
                }
                dataIndex++;
            }
        });

        var popped = percentageData.pop();
        while (!popped && percentageData.length) {
            tempDates.pop();
            popped = percentageData.pop();
        }
        percentageData.push(popped);

        if (!chartGroup.percentFatalChart) {
            var percentage = {
                data: percentageData,
                backgroundColor: colors[0],
                borderColor: colors[0]
            };
            chartGroup.percentFatalChart = CreateBarPlot('percentFatalChart' + chartGroup.groupIndex, tempDates, [percentage], 'Death Percentage');
            chartGroup.percentFatalChart.options.scales.yAxes[0].ticks.max = 25;
            chartGroup.percentFatalChart.options.scales.yAxes[0].scaleLabel.labelString = 'Percent Fatal'
            chartGroup.percentFatalChart.options.scales.yAxes[0].scaleLabel.display = true;
            chartGroup.percentFatalChart.update();
        }
        else {
            chartGroup.percentFatalChart.data.labels = tempDates;
            chartGroup.percentFatalChart.data.datasets[0].data = percentageData;
            chartGroup.percentFatalChart.update();
        }
    }

    function GetStateData(chartGroup) {
        $.getJSON(stateDataUrl + '?state=' + chartGroup.selectedState + '&country=' + chartGroup.selectedCountry.name).done(result => {

            if (!result.success) {
                console.error('State: ' + result.errorMessage);
                return;
            }

            dates = result.dates;
            chartGroup.confirmedData = result.confirmedCases.data;

            var title = chartGroup.selectedState + ' New Confirmed Cases';

            if (!chartGroup.confirmedChart) {
                var confirmed = {
                    data: chartGroup.confirmedData,
                    backgroundColor: colors[7],
                    borderColor: colors[7]
                };
                chartGroup.confirmedChart = CreateBarPlot('stateConfirmedChart' + chartGroup.groupIndex, result.dates, [confirmed], title);
            } else {
                UpdateChart(chartGroup.confirmedChart, chartGroup.confirmedData, title);
            }

            chartGroup.deathData = result.deaths.data;

            title = chartGroup.selectedState + ' New Deaths';

            if (!chartGroup.deathChart) {
                var deaths = {
                    data: chartGroup.deathData,
                    backgroundColor: colors[8],
                    borderColor: colors[8]
                };
                chartGroup.deathChart = CreateBarPlot('stateDeathsChart' + chartGroup.groupIndex, result.dates, [deaths], title);
            } else {
                UpdateChart(chartGroup.deathChart, chartGroup.deathData, title);
            }

            var percentInfectedData = [];
            var total = 0;
            result.confirmedCases.data.forEach((x, i) => {
                total += x;
                percentInfectedData[i] = total / result.deaths.population * 100;
            });

            title = chartGroup.selectedState + ' Percent of Population That Has Been (Confirmed) Infected';

            if (!chartGroup.percentInfectedChart) {
                var percentInfectedDataSet = [
                    {
                        data: percentInfectedData,
                        backgroundColor: colors[14],
                        borderColor: colors[14]
                    }
                ];
                chartGroup.percentInfectedChart = CreateBarPlot('percentInfectedChart' + chartGroup.groupIndex, result.dates, percentInfectedDataSet, title, 'line');
            } else {
                UpdateChart(chartGroup.percentInfectedChart, percentInfectedData, title);
            }

            UpdatePercentFatalChart(chartGroup);
        });
    }

    $.getJSON(countryListUrl).done(result => {
        if (!result.success) {
            console.error('Country: ' + result.errorMessage);
            return;
        }
        self = {
            countries: result.countries,
            selectedCountry: {},
            chartGroups: [{ groupIndex: 0, selectedCountry: {}, selectedState: {}, offset: 15, range: 5 },
            { groupIndex: 1, selectedCountry: {}, selectedState: {}, offset: 15, range: 5 }],
            updateCharts: GetStateData,
            updateFatality: UpdatePercentFatalChart
        };

        ko.track(self);
        ko.track(self.chartGroups[0]);
        ko.track(self.chartGroups[1]);
        self.countries.forEach(x => { if (x.name == 'US') self.chartGroups[0].selectedCountry = x; });
        self.countries.forEach(x => { if (x.name == 'US') self.chartGroups[1].selectedCountry = x; });

        ko.applyBindings(self);
        self.chartGroups[0].selectedState = 'Alabama';
        GetStateData(self.chartGroups[0]);
        self.chartGroups[1].selectedState = 'Georgia';
        GetStateData(self.chartGroups[1]);
    });
});