// == NOTE ==
// This code uses ES6 promises. If you want to use this code in a browser
// that doesn't supporting promises natively, you'll have to include a polyfill.
var idConst = '';
var accessToken = accessToken || "";

gapi.analytics.ready(function () {
    /*
     * Authorize the user immediately if the user has already granted access.
     * If no access has been created, render an authorize button inside the
     * element with the ID "embed-api-auth-container".
     */
    gapi.analytics.auth.authorize({
        container: 'embed-api-auth-container',
        scopes: ['https://www.googleapis.com/auth/analytics.manage.users'],
        serverAuth: {
            access_token: accessToken
        }
    });

    /**
     * Create a new ActiveUsers instance to be rendered inside of an
     * element with the id "active-users-container" and poll for changes every
     * five seconds.
     */
    var activeUsers = new gapi.analytics.ext.ActiveUsers({
        container: 'active-users-container',
        pollingInterval: 5
    });


    /*
     * Add CSS animation to visually show the when users come and go.
     */
    activeUsers.once('success', function () {
        var timeout;

        this.on('change', function (data) {
            var element = this.container.firstChild;
            var animationClass = data.delta > 0 ? 'is-increasing' : 'is-decreasing';
            element.className += (' ' + animationClass);

            clearTimeout(timeout);
            timeout = setTimeout(function () {
                element.className =
                    element.className.replace(/ is-(increasing|decreasing)/g, '');
            }, 3000);
        });
    });


    /*
     * Create a new ViewSelector2 instance to be rendered inside of an
     * element with the id "view-selector-container".
     */
    var viewSelector = new gapi.analytics.ext.ViewSelector2({
        container: 'view-selector-container',
    })
    .execute();

    var now = moment();
    var dateRange = {
        'start-date': "7daysAgo",
        'end-date': "yesterday"
    };

    /*
     * Update the activeUsers component, the Chartjs charts, and the dashboard
     * title whenever the user changes the view.
     */
    viewSelector.on('viewChange', function (data) {
        //var title = document.getElementById('view-name');
        //title.innerHTML = data.property.name + ' (' + data.view.name + ')';

        // Start tracking active users for this view.
        activeUsers.set(data).execute();
        idConst = data.ids;
        // Render all the of charts for this view.
        renderYearOverYearChart(data.ids, moment(now).subtract(7, 'day').day(0).format('YYYY-MM-DD'), moment(now).format('YYYY-MM-DD'));
        $(".ViewSelector2 .FormField").select2();
    });

    /*
     * Create a new DateRangeSelector instance to be rendered inside of an
     * element with the id "date-range-selector-1-container", set its date range
     * and then render it to the page.
     */
    var dateRangeSelector = new gapi.analytics.ext.DateRangeSelector({
        container: 'date-range-container'
    })
    .set(dateRange)
    .execute();
    
    $("input[type=date]").datepicker({
        format: 'yyyy-mm-dd',
        autoclose: true
    });

    dateRangeSelector.on('change', function (data) {
        renderYearOverYearChart(idConst, data["start-date"], data["end-date"]);
    });

    /*
     * Draw the a chart.js bar chart with data from the specified view that
     * overlays session data for the current year over session data for the
     * previous year, grouped by month.
     */
    function renderYearOverYearChart(ids, startDate, endDate) {
        var resusers = query({
            ids: ids,
            metrics: 'ga:users',
            dimensions: 'ga:date,ga:nthDay',
            'start-date': startDate,
            'end-date': endDate
        });

        var resSessions = query({
            ids: ids,
            metrics: 'ga:sessions',
            dimensions: 'ga:date,ga:nthDay',
            'start-date': startDate,
            'end-date': endDate
        });


        var resMetrics = query({
            ids: ids,
            metrics: 'ga:sessions,ga:users,ga:visits,ga:pageviews,ga:percentNewVisits,ga:avgTimeOnSite,ga:entranceBounceRate,ga:exitRate,ga:pageviewsPerVisit,ga:avgPageLoadTime',
            'start-date': startDate,
            'end-date': endDate
        });


        var resPageViews = query({
            ids: ids,
            metrics: 'ga:pageviews',
            dimensions: 'ga:date,ga:nthDay',
            'start-date': startDate,
            'end-date': endDate
        });

        var resPagePerSession = query({
            ids: ids,
            metrics: 'ga:pageviewsPerVisit',
            dimensions: 'ga:date,ga:nthDay',
            'start-date': startDate,
            'end-date': endDate
        });

        var resAvgSession = query({
            ids: ids,
            metrics: 'ga:avgTimeOnSite',
            dimensions: 'ga:date,ga:nthDay',
            'start-date': startDate,
            'end-date': endDate
        });

        var resBounceRate = query({
            ids: ids,
            metrics: 'ga:entranceBounceRate',
            dimensions: 'ga:date,ga:nthDay',
            'start-date': startDate,
            'end-date': endDate
        });

        var resPercNewVisits = query({
            ids: ids,
            metrics: 'ga:percentNewVisits',
            dimensions: 'ga:date,ga:nthDay',
            'start-date': startDate,
            'end-date': endDate
        });

        Promise.all([resusers, resMetrics, resSessions, resPageViews, resPagePerSession, resAvgSession, resBounceRate, resPercNewVisits]).then(function (results) {

            var sessions = 0;
            var users = 0;
            var pageviews = 0;
            var pagesSession = 0;
            var avgSessionDuration = 0;
            var bounceRate = 0;


            var newVisits = 0;
            var returningVisits = 0;

            var dataUsers = results[0].rows.map(function (row) { return +row[2]; });
            var labelUsers = results[0].rows.map(function (row) { return +row[0]; });

            var dataSessions = results[2].rows.map(function (row) { return +row[2]; });
            var labelSessions = results[2].rows.map(function (row) { return +row[0]; });

            var dataPageViews = results[3].rows.map(function (row) { return +row[2]; });
            var labelPageViews = results[3].rows.map(function (row) { return +row[0]; });

            var dataPagePerSession = results[4].rows.map(function (row) { return +row[2]; });
            var labelPagePerSession = results[4].rows.map(function (row) { return +row[0]; });

            var dataAvgSession = results[5].rows.map(function (row) { return +row[2]; });
            var labelAvgSession = results[5].rows.map(function (row) { return +row[0]; });

            var dataBounceRate = results[6].rows.map(function (row) { return +row[2]; });
            var labelBounceRate = results[6].rows.map(function (row) { return +row[0]; });

            var dataPercNewVisits = results[7].rows.map(function (row) { return +row[2]; });
            var labelPercNewVisits = results[7].rows.map(function (row) { return +row[0]; });

            if (results[1].rows != undefined) {
                var duration = moment.duration(parseInt(results[1].rows[0][5], 10));
                var addZero = function (v) { return (v < 10 ? '0' : '') + Math.floor(v); };

                var time = addZero(duration.hours()) +
                     ':' + addZero(duration.minutes()) +
                     ':' + addZero(duration.seconds()) +
                     ':' + addZero(duration.milliseconds());

                sessions = results[1].rows[0][0];
                users = results[1].rows[0][1];
                pageviews = Math.round(results[1].rows[0][3] * 100) / 100;
                avgSessionDuration = time;
                bounceRate = Math.round(results[1].rows[0][6] * 100) / 100;
                pagesSession = Math.round(results[1].rows[0][8] * 100) / 100;
                newVisits = Math.round(results[1].rows[0][4] * 100) / 100;
                returningVisits = 100 - Math.round(results[1].rows[0][4] * 100) / 100;
            }
            returningVisits = Math.round(returningVisits);


            $("#spanSessions").html(sessions.toString());
            $("#spanUsers").html(users.toString());
            $("#spanPageViews").html(pageviews.toString());
            $("#pageSessions").html(pagesSession.toString());
            $("#spanAvgSessionDuration").html(avgSessionDuration.toString());
            $("#spanBounceRate").html(bounceRate.toString() + '%');
            $("#spannewsessions").html(newVisits.toString() + '%');

            labelUsers = labelUsers.map(function (label) {
                return moment(label, 'YYYYMMDD').format('MMM D');
            });

            labelSessions = labelSessions.map(function (label) {
                return moment(label, 'YYYYMMDD').format('MMM D');
            });

            labelPageViews = labelPageViews.map(function (label) {
                return moment(label, 'YYYYMMDD').format('MMM D');
            });

            labelAvgSession = labelAvgSession.map(function (label) {
                return moment(label, 'YYYYMMDD').format('MMM D');
            });

            labelBounceRate = labelBounceRate.map(function (label) {
                return moment(label, 'YYYYMMDD').format('MMM D');
            });

            labelPagePerSession = labelPagePerSession.map(function (label) {
                return moment(label, 'YYYYMMDD').format('MMM D');
            });

            labelPercNewVisits = labelPercNewVisits.map(function (label) {
                return moment(label, 'YYYYMMDD').format('MMM D');
            });

            var dataUserChart = {
                labels: labelUsers,
                datasets: [
                  {
                      label: 'Users',
                      fillColor: "#E5F3F9",
                      strokeColor: "#058DC7",
                      pointColor: "#058DC7",
                      pointStrokeColor: "#fff",
                      data: dataUsers
                  }
                ]
            };

            new Chart(makeCanvas('chart-2-container')).Line(dataUserChart);
            generateLegend('legend-2-container', dataUserChart.datasets);

            var dataSessionChart = {
                labels: labelSessions,
                datasets: [
                  {
                      label: 'Sessions',
                      fillColor: "#E5F3F9",
                      strokeColor: "#058DC7",
                      pointColor: "#058DC7",
                      pointStrokeColor: "#fff",
                      data: dataSessions
                  }
                ]
            };

            new Chart(makeCanvas('chart-4-container')).Line(dataSessionChart);
            generateLegend('legend-4-container', dataSessionChart.datasets);


            var dataPageViewChart = {
                labels: labelPageViews,
                datasets: [
                  {
                      label: 'Page Views',
                      fillColor: "#E5F3F9",
                      strokeColor: "#058DC7",
                      pointColor: "#058DC7",
                      pointStrokeColor: "#fff",
                      data: dataPageViews
                  }
                ]
            };

            new Chart(makeCanvas('chart-6-container')).Line(dataPageViewChart);
            generateLegend('legend-6-container', dataPageViewChart.datasets);


            var dataPagePerSessionChart = {
                labels: labelPagePerSession,
                datasets: [
                  {
                      label: 'Pages / Session',
                      fillColor: "#E5F3F9",
                      strokeColor: "#058DC7",
                      pointColor: "#058DC7",
                      pointStrokeColor: "#fff",
                      data: dataPagePerSession
                  }
                ]
            };

            new Chart(makeCanvas('chart-7-container')).Line(dataPagePerSessionChart);
            generateLegend('legend-7-container', dataPagePerSessionChart.datasets);


            var dataAvgSessionChart = {
                labels: labelAvgSession,
                datasets: [
                  {
                      label: 'Avg. Session Duration',
                      fillColor: "#E5F3F9",
                      strokeColor: "#058DC7",
                      pointColor: "#058DC7",
                      pointStrokeColor: "#fff",
                      data: dataAvgSession
                  }
                ]
            };

            new Chart(makeCanvas('chart-8-container')).Line(dataAvgSessionChart);
            generateLegend('legend-8-container', dataAvgSessionChart.datasets);

            var dataBounceRateChart = {
                labels: labelBounceRate,
                datasets: [
                  {
                      label: 'Bounce Rate',
                      fillColor: "#E5F3F9",
                      strokeColor: "#058DC7",
                      pointColor: "#058DC7",
                      pointStrokeColor: "#fff",
                      data: dataBounceRate
                  }
                ]
            };

            new Chart(makeCanvas('chart-9-container')).Line(dataBounceRateChart);
            generateLegend('legend-9-container', dataBounceRateChart.datasets);

            var dataPercNewVisitChart = {
                labels: labelPercNewVisits,
                datasets: [
                  {
                      label: 'Bounce Rate',
                      fillColor: "#E5F3F9",
                      strokeColor: "#058DC7",
                      pointColor: "#058DC7",
                      pointStrokeColor: "#fff",
                      data: dataPercNewVisits
                  }
                ]
            };

            new Chart(makeCanvas('chart-10-container')).Line(dataPercNewVisitChart);
            generateLegend('legend-10-container', dataPercNewVisitChart.datasets);


            var dataPie = [];

            dataPie.push({
                label: "New Visitor",
                value: newVisits,
                color: '#058DC7'
            });

            dataPie.push({
                label: "Returning Visitor",
                value: returningVisits,
                color: '#50B432'
            });

            new Chart(makeCanvas('chart-3-container')).Pie(dataPie);
            generateLegend('legend-3-container', dataPie);
        });
    }

    /*
     * Extend the Embed APIs `gapi.analytics.report.Data` component to
     * return a promise the is fulfilled with the value returned by the API.
     */
    function query(params) {
        return new Promise(function (resolve, reject) {
            var data = new gapi.analytics.report.Data({ query: params });
            data.once('success', function (response) { resolve(response); })
                .once('error', function (response) { reject(response); })
                .execute();
        });
    }


    /*
     * Create a new canvas inside the specified element. Set it to be the width
     * and height of its container.
    */
    function makeCanvas(id) {
        var container = document.getElementById(id);
        var canvas = document.createElement('canvas');
        var ctx = canvas.getContext('2d');

        container.innerHTML = '';
        canvas.width = container.offsetWidth;
        canvas.height = 200;
        container.appendChild(canvas);

        return ctx;
    }


    /*
     * Create a visual legend inside the specified element based off of a
     * Chart.js dataset.
     */
    function generateLegend(id, items) {
        var legend = document.getElementById(id);
        legend.innerHTML = items.map(function (item) {
            var color = item.color || item.fillColor;
            var label = item.label;
            return '<li><i style="background:' + color + '"></i>' + label + '</li>';
        }).join('');
    }


    // Set some global Chart.js defaults.
    Chart.defaults.global.animationSteps = 60;
    Chart.defaults.global.animationEasing = 'easeInOutQuart';
    Chart.defaults.global.responsive = true;
    Chart.defaults.global.maintainAspectRatio = false;

});