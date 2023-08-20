new Chartist.Line('#pumpRotationalSpeedChart', {
    labels: [1, 2, 3, 4, 5, 6, 7, 8, 9],
    series: [[100, 120, 180, 200]]
});

new Chartist.Line('#reservoirLevelChart', {
    labels: [1, 2, 3, 4],
    series: [[5, 2, 8, 3]]
});

let connection = new signalR.HubConnectionBuilder()
    .withUrl("/model")
    .build();

connection.start().then(
    function () {

        startStreaming();

    });

function startStreaming() {
    connection.stream("StreamResults").subscribe({
        close: false,
        next: displayResults,
        error: function (err) {
            logger.log(err);
        }
    });
}

function displayResults(result) {
    logger.log(result);
}
